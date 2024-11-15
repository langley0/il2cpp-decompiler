using System.Diagnostics;
using ILSpy.IO;
using ILSpy.Enums;

namespace ILSpy.Meta
{
	public sealed partial class TablesStream : DotNetStream
	{
		bool initialized;
		uint reserved1;
		byte majorVersion;
		byte minorVersion;
		MetadataStreamFlags flags;
		byte log2Rid;
		ulong validMask;
		ulong sortedMask;
		uint extraData;
		Table[]? mdTables;
		uint mdTablesPos;

		IColumnReader columnReader;
		IRowReader<RawMethodRow> methodRowReader;
		readonly CLRRuntimeReaderKind runtime;

		public Table ModuleTable { get; private set; }
		public Table TypeRefTable { get; private set; }
		public Table TypeDefTable { get; private set; }
		public Table FieldPtrTable { get; private set; }
		public Table FieldTable { get; private set; }
		public Table MethodPtrTable { get; private set; }
		public Table MethodTable { get; private set; }
		public Table ParamPtrTable { get; private set; }
		public Table ParamTable { get; private set; }
		public Table InterfaceImplTable { get; private set; }
		public Table MemberRefTable { get; private set; }
		public Table ConstantTable { get; private set; }
		public Table CustomAttributeTable { get; private set; }
		public Table FieldMarshalTable { get; private set; }
		public Table DeclSecurityTable { get; private set; }
		public Table ClassLayoutTable { get; private set; }
		public Table FieldLayoutTable { get; private set; }
		public Table StandAloneSigTable { get; private set; }
		public Table EventMapTable { get; private set; }
		public Table EventPtrTable { get; private set; }
		public Table EventTable { get; private set; }
		public Table PropertyMapTable { get; private set; }
		public Table PropertyPtrTable { get; private set; }
		public Table PropertyTable { get; private set; }
		public Table MethodSemanticsTable { get; private set; }
		public Table MethodImplTable { get; private set; }
		public Table ModuleRefTable { get; private set; }
		public Table TypeSpecTable { get; private set; }
		public Table ImplMapTable { get; private set; }
		public Table FieldRVATable { get; private set; }
		public Table ENCLogTable { get; private set; }
		public Table ENCMapTable { get; private set; }
		public Table AssemblyTable { get; private set; }
		public Table AssemblyProcessorTable { get; private set; }
		public Table AssemblyOSTable { get; private set; }
		public Table AssemblyRefTable { get; private set; }
		public Table AssemblyRefProcessorTable { get; private set; }
		public Table AssemblyRefOSTable { get; private set; }
		public Table FileTable { get; private set; }
		public Table ExportedTypeTable { get; private set; }
		public Table ManifestResourceTable { get; private set; }
		public Table NestedClassTable { get; private set; }
		public Table GenericParamTable { get; private set; }
		public Table MethodSpecTable { get; private set; }
		public Table GenericParamConstraintTable { get; private set; }
		public Table DocumentTable { get; private set; }
		public Table MethodDebugInformationTable { get; private set; }
		public Table LocalScopeTable { get; private set; }
		public Table LocalVariableTable { get; private set; }
		public Table LocalConstantTable { get; private set; }
		public Table ImportScopeTable { get; private set; }
		public Table StateMachineMethodTable { get; private set; }
		public Table CustomDebugInformationTable { get; private set; }

		public IColumnReader ColumnReader
		{
			get => columnReader;
			set => columnReader = value;
		}

		public IRowReader<RawMethodRow> MethodRowReader
		{
			get => methodRowReader;
			set => methodRowReader = value;
		}

		public uint Reserved1 => reserved1;

		public ushort Version => (ushort)((majorVersion << 8) | minorVersion);

		public MetadataStreamFlags Flags => flags;

		public byte Log2Rid => log2Rid;

		public ulong ValidMask => validMask;

		public ulong SortedMask => sortedMask;

		public uint ExtraData => extraData;

		public Table[]? Tables => mdTables;

		public bool HasBigStrings => (flags & MetadataStreamFlags.BigStrings) != 0;

		public bool HasBigGUID => (flags & MetadataStreamFlags.BigGUID) != 0;

		public bool HasBigBlob => (flags & MetadataStreamFlags.BigBlob) != 0;

		public bool HasPadding => runtime == CLRRuntimeReaderKind.CLR && (flags & MetadataStreamFlags.Padding) != 0;

		public bool HasDeltaOnly => runtime == CLRRuntimeReaderKind.CLR && (flags & MetadataStreamFlags.DeltaOnly) != 0;

		public bool HasExtraData => runtime == CLRRuntimeReaderKind.CLR && (flags & MetadataStreamFlags.ExtraData) != 0;

		public bool HasDelete => runtime == CLRRuntimeReaderKind.CLR && (flags & MetadataStreamFlags.HasDelete) != 0;

		public TablesStream(DataReaderFactory mdReaderFactory, uint metadataBaseOffset, StreamHeader streamHeader)
			: this(mdReaderFactory, metadataBaseOffset, streamHeader, CLRRuntimeReaderKind.CLR)
		{
		}
#pragma warning disable CS8618
		public TablesStream(DataReaderFactory mdReaderFactory, uint metadataBaseOffset, StreamHeader streamHeader, CLRRuntimeReaderKind runtime)
			: base(mdReaderFactory, metadataBaseOffset, streamHeader)
		{
			this.runtime = runtime;
		}
#pragma warning restore CS8618

		internal void Initialize(uint[]? typeSystemTableRows, bool forceAllBig = false)
		{
			if (initialized)
			{
				throw new Exception("Initialize() has already been called");
			}
			initialized = true;

			var reader = dataReader ?? throw new InvalidOperationException("DataReader is null");
			reserved1 = reader.ReadUInt32();
			majorVersion = reader.ReadByte();
			minorVersion = reader.ReadByte();
			flags = (MetadataStreamFlags)reader.ReadByte();
			log2Rid = reader.ReadByte();
			validMask = reader.ReadUInt64();
			sortedMask = reader.ReadUInt64();
			// Mono assumes everything is sorted
			if (runtime == CLRRuntimeReaderKind.Mono)
			{
				sortedMask = ulong.MaxValue;
			}

			var dnTableSizes = new DotNetTableSizes();
			byte tmpMajor = majorVersion, tmpMinor = minorVersion;
			// It ignores the version so use 2.0
			if (runtime == CLRRuntimeReaderKind.Mono)
			{
				tmpMajor = 2;
				tmpMinor = 0;
			}
			var tableInfos = dnTableSizes.CreateTables(tmpMajor, tmpMinor, out int maxPresentTables);
			if (typeSystemTableRows is not null)
			{
				maxPresentTables = DotNetTableSizes.normalMaxTables;
			}
			mdTables = new Table[tableInfos.Length];

			ulong valid = validMask;
			var sizes = new uint[64];
			for (int i = 0; i < 64; valid >>= 1, i++)
			{
				uint rows = (valid & 1) == 0 ? 0 : reader.ReadUInt32();
				// Mono ignores the high byte
				rows &= 0x00FFFFFF;
				if (i >= maxPresentTables)
					rows = 0;
				sizes[i] = rows;
				if (i < mdTables.Length)
					mdTables[i] = new Table((TableType)i, rows, tableInfos[i]);
			}

			if (HasExtraData)
				extraData = reader.ReadUInt32();

			var debugSizes = sizes;
			if (typeSystemTableRows is not null)
			{
				debugSizes = new uint[sizes.Length];
				for (int i = 0; i < 64; i++)
				{
					if (DotNetTableSizes.IsSystemTable((TableType)i))
						debugSizes[i] = typeSystemTableRows[i];
					else
						debugSizes[i] = sizes[i];
				}
			}

			dnTableSizes.InitializeSizes(HasBigStrings, HasBigGUID, HasBigBlob, sizes, debugSizes, forceAllBig);

			mdTablesPos = reader.Position;
			InitializeMdTableReaders();
			InitializeTables();
		}

		/// <inheritdoc/>
		protected override void OnReaderRecreated() => InitializeMdTableReaders();

		void InitializeMdTableReaders()
		{
			var reader = dataReader ?? throw new InvalidOperationException("DataReader is null");
			reader.Position = mdTablesPos;
			var currentPos = reader.Position;
			foreach (var mdTable in mdTables ?? [])
			{
				var dataLen = (uint)(mdTable.TableInfo?.RowSize ?? throw new InvalidOperationException("TableInfo is null")) * mdTable.Rows;
				if (currentPos > reader.Length)
				{
					currentPos = reader.Length;
				}
				if ((ulong)currentPos + dataLen > reader.Length)
				{
					dataLen = reader.Length - currentPos;
				}
				mdTable.DataReader = reader.Slice(currentPos, dataLen);
				var newPos = currentPos + dataLen;
				if (newPos < currentPos)
				{
					throw new BadImageFormatException("Too big MD table");
				}
				currentPos = newPos;
			}
		}

		void InitializeTables()
		{
			if (mdTables is null)
			{
				throw new InvalidOperationException("mdTables is null");
			}

			ModuleTable = mdTables[(int)TableType.Module];
			TypeRefTable = mdTables[(int)TableType.TypeRef];
			TypeDefTable = mdTables[(int)TableType.TypeDef];
			FieldPtrTable = mdTables[(int)TableType.FieldPtr];
			FieldTable = mdTables[(int)TableType.Field];
			MethodPtrTable = mdTables[(int)TableType.MethodPtr];
			MethodTable = mdTables[(int)TableType.Method];
			ParamPtrTable = mdTables[(int)TableType.ParamPtr];
			ParamTable = mdTables[(int)TableType.Param];
			InterfaceImplTable = mdTables[(int)TableType.InterfaceImpl];
			MemberRefTable = mdTables[(int)TableType.MemberRef];
			ConstantTable = mdTables[(int)TableType.Constant];
			CustomAttributeTable = mdTables[(int)TableType.CustomAttribute];
			FieldMarshalTable = mdTables[(int)TableType.FieldMarshal];
			DeclSecurityTable = mdTables[(int)TableType.DeclSecurity];
			ClassLayoutTable = mdTables[(int)TableType.ClassLayout];
			FieldLayoutTable = mdTables[(int)TableType.FieldLayout];
			StandAloneSigTable = mdTables[(int)TableType.StandAloneSig];
			EventMapTable = mdTables[(int)TableType.EventMap];
			EventPtrTable = mdTables[(int)TableType.EventPtr];
			EventTable = mdTables[(int)TableType.Event];
			PropertyMapTable = mdTables[(int)TableType.PropertyMap];
			PropertyPtrTable = mdTables[(int)TableType.PropertyPtr];
			PropertyTable = mdTables[(int)TableType.Property];
			MethodSemanticsTable = mdTables[(int)TableType.MethodSemantics];
			MethodImplTable = mdTables[(int)TableType.MethodImpl];
			ModuleRefTable = mdTables[(int)TableType.ModuleRef];
			TypeSpecTable = mdTables[(int)TableType.TypeSpec];
			ImplMapTable = mdTables[(int)TableType.ImplMap];
			FieldRVATable = mdTables[(int)TableType.FieldRVA];
			ENCLogTable = mdTables[(int)TableType.ENCLog];
			ENCMapTable = mdTables[(int)TableType.ENCMap];
			AssemblyTable = mdTables[(int)TableType.Assembly];
			AssemblyProcessorTable = mdTables[(int)TableType.AssemblyProcessor];
			AssemblyOSTable = mdTables[(int)TableType.AssemblyOS];
			AssemblyRefTable = mdTables[(int)TableType.AssemblyRef];
			AssemblyRefProcessorTable = mdTables[(int)TableType.AssemblyRefProcessor];
			AssemblyRefOSTable = mdTables[(int)TableType.AssemblyRefOS];
			FileTable = mdTables[(int)TableType.File];
			ExportedTypeTable = mdTables[(int)TableType.ExportedType];
			ManifestResourceTable = mdTables[(int)TableType.ManifestResource];
			NestedClassTable = mdTables[(int)TableType.NestedClass];
			GenericParamTable = mdTables[(int)TableType.GenericParam];
			MethodSpecTable = mdTables[(int)TableType.MethodSpec];
			GenericParamConstraintTable = mdTables[(int)TableType.GenericParamConstraint];
			DocumentTable = mdTables[(int)TableType.Document];
			MethodDebugInformationTable = mdTables[(int)TableType.MethodDebugInformation];
			LocalScopeTable = mdTables[(int)TableType.LocalScope];
			LocalVariableTable = mdTables[(int)TableType.LocalVariable];
			LocalConstantTable = mdTables[(int)TableType.LocalConstant];
			ImportScopeTable = mdTables[(int)TableType.ImportScope];
			StateMachineMethodTable = mdTables[(int)TableType.StateMachineMethod];
			CustomDebugInformationTable = mdTables[(int)TableType.CustomDebugInformation];
		}

		/// <inheritdoc/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				var mt = mdTables;
				if (mt is not null)
				{
					foreach (var mdTable in mt)
					{
						if (mdTable is not null)
							mdTable.Dispose();
					}
					mdTables = null;
				}
			}
			base.Dispose(disposing);
		}

		public Table? Get(TableType table)
		{
			if (mdTables is null)
			{
				return null;
			}

			int index = (int)table;
			if ((uint)index >= (uint)mdTables.Length)
			{
				return null;
			}
			return mdTables[index];
		}

		public bool HasTable(TableType table) => (uint)table < mdTables?.Length;

		public bool IsSorted(Table table)
		{
			int index = (int)table.TableType;
			if ((uint)index >= 64)
			{
				return false;
			}
			return (sortedMask & (1UL << index)) != 0;
		}



		internal bool TryReadColumn24(Table? table, uint rid, int colIndex, out uint value) =>
			TryReadColumn24(table, rid, table?.TableInfo?.Columns[colIndex], out value);

		internal bool TryReadColumn24(Table? table, uint rid, ColumnInfo? column, out uint value)
		{
			if (table is null || column is null)
			{
				throw new ArgumentNullException(table is null ? nameof(table) : nameof(column));
			}
			if (table.DataReader is null || table.TableInfo is null)
			{
				throw new ArgumentNullException("table.DataReader or table.TableInfo is null");
			}

			Debug.Assert(column.Size == 2 || column.Size == 4);
			if (table.IsInvalidRID(rid))
			{
				value = 0;
				return false;
			}
			var cr = columnReader;
			if (cr is not null && cr.ReadColumn(table, rid, column, out value))
			{
				return true;
			}
			var reader = table.DataReader;
			reader.Position = (rid - 1) * (uint)table.TableInfo.RowSize + (uint)column.Offset;
			value = column.Size == 2 ? reader.ReadUInt16() : reader.ReadUInt32();
			return true;
		}

		public bool TryReadNestedClassRow(uint rid, out RawNestedClassRow row)
		{
			var table = NestedClassTable;
			if (table.IsInvalidRID(rid))
			{
				row = default;
				return false;
			}
			var reader = table.DataReader ?? throw new InvalidOperationException("DataReader is null");
			var tableInfo = table.TableInfo ?? throw new InvalidOperationException("TableInfo is null");

			reader.Position = (rid - 1) * (uint)tableInfo.RowSize;
			row = new RawNestedClassRow(
				table.Column0!.Read24(ref reader),
				table.Column1!.Read24(ref reader));
			return true;
		}

		public bool TryReadTypeDefRow(uint rid, out RawTypeDefRow row)
		{
			var table = TypeDefTable;
			if (table.IsInvalidRID(rid))
			{
				row = default;
				return false;
			}
			var tableInfo = table.TableInfo;
			if (tableInfo is null)
			{
				row = default;
				return false;
			}

			var reader = table.DataReader ?? throw new InvalidOperationException("DataReader is null");

			reader.Position = (rid - 1) * (uint)tableInfo.RowSize;
			row = new RawTypeDefRow(
				reader.ReadUInt32(),
				table.Column1?.Read24(ref reader) ?? throw new InvalidOperationException("Column1 is null"),
				table.Column2?.Read24(ref reader) ?? throw new InvalidOperationException("Column2 is null"),
				table.Column3?.Read24(ref reader) ?? throw new InvalidOperationException("Column3 is null"),
				table.Column4?.Read24(ref reader) ?? throw new InvalidOperationException("Column4 is null"),
				table.Column5?.Read24(ref reader) ?? throw new InvalidOperationException("Column5 is null"));
			return true;
		}

		public bool TryReadExportedTypeRow(uint rid, out RawExportedTypeRow row)
		{
			var table = ExportedTypeTable;
			if (table.IsInvalidRID(rid))
			{
				row = default;
				return false;
			}

			var reader = table.DataReader ?? throw new Exception("DataReader is null");
			var tableInfo = table.TableInfo ?? throw new Exception("TableInfo is null");

			reader.Position = (rid - 1) * (uint)tableInfo.RowSize;
			row = new RawExportedTypeRow(
				reader.ReadUInt32(),
				reader.ReadUInt32(),
				table.Column2?.Read24(ref reader) ?? throw new Exception("Column2 is null"),
				table.Column3?.Read24(ref reader) ?? throw new Exception("Column3 is null"),
				table.Column4?.Read24(ref reader) ?? throw new Exception("Column4 is null"));
			return true;
		}

		public bool TryReadFieldRow(uint rid, out RawFieldRow row)
		{
			var table = FieldTable;
			if (table.IsInvalidRID(rid))
			{
				row = default;
				return false;
			}

			Debug.Assert(table.Column1 is not null);
			Debug.Assert(table.Column2 is not null);
			Debug.Assert(table.DataReader is not null);
			Debug.Assert(table.TableInfo is not null);

			var reader = table.DataReader;
			reader.Position = (rid - 1) * (uint)table.TableInfo.RowSize;
			row = new RawFieldRow(
				reader.ReadUInt16(),
				table.Column1.Read24(ref reader),
				table.Column2.Read24(ref reader));
			return true;
		}

		public bool TryReadMethodRow(uint rid, out RawMethodRow row)
		{
			var table = MethodTable;
			if (table.IsInvalidRID(rid))
			{
				row = default;
				return false;
			}
			var mrr = methodRowReader;
			if (mrr is not null && mrr.TryReadRow(rid, out row))
			{
				return true;
			}


			Debug.Assert(table.Column3 is not null);
			Debug.Assert(table.Column4 is not null);
			Debug.Assert(table.Column5 is not null);
			Debug.Assert(table.DataReader is not null);
			Debug.Assert(table.TableInfo is not null);

			var reader = table.DataReader;
			reader.Position = (rid - 1) * (uint)table.TableInfo.RowSize;
			row = new RawMethodRow(
				reader.ReadUInt32(),
				reader.ReadUInt16(),
				reader.ReadUInt16(),
				table.Column3.Read24(ref reader),
				table.Column4.Read24(ref reader),
				table.Column5.Read24(ref reader));
			return true;
		}

		public bool TryReadEventRow(uint rid, out RawEventRow row)
		{
			var table = EventTable;
			if (table.IsInvalidRID(rid))
			{
				row = default;
				return false;
			}

			Debug.Assert(table.Column1 is not null);
			Debug.Assert(table.Column2 is not null);
			Debug.Assert(table.DataReader is not null);
			Debug.Assert(table.TableInfo is not null);

			var reader = table.DataReader;
			reader.Position = (rid - 1) * (uint)table.TableInfo.RowSize;
			row = new RawEventRow(
				reader.ReadUInt16(),
				table.Column1.Read24(ref reader),
				table.Column2.Read24(ref reader));
			return true;
		}

		public bool TryReadPropertyRow(uint rid, out RawPropertyRow row)
		{
			var table = PropertyTable;
			if (table.IsInvalidRID(rid))
			{
				row = default;
				return false;
			}

			Debug.Assert(table.Column1 is not null);
			Debug.Assert(table.Column2 is not null);
			Debug.Assert(table.DataReader is not null);
			Debug.Assert(table.TableInfo is not null);

			var reader = table.DataReader;
			reader.Position = (rid - 1) * (uint)table.TableInfo.RowSize;
			row = new RawPropertyRow(
				reader.ReadUInt16(),
				table.Column1.Read24(ref reader),
				table.Column2.Read24(ref reader));
			return true;
		}

	}
}