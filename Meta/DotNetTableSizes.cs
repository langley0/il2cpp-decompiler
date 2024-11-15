namespace ILSpy.Meta
{
	public sealed class DotNetTableSizes
	{
		bool bigStrings;
		bool bigGuid;
		bool bigBlob;
		bool forceAllBig;
		TableInfo[]? tableInfos;

		internal static bool IsSystemTable(TableType tableType) => tableType < TableType.Document;

		internal void InitializeSizes(bool bigStrings, bool bigGuid, bool bigBlob, IList<uint> systemRowCounts, IList<uint> debugRowCounts, bool forceAllBig = false)
		{
			this.bigStrings = bigStrings || forceAllBig;
			this.bigGuid = bigGuid || forceAllBig;
			this.bigBlob = bigBlob || forceAllBig;
			this.forceAllBig = forceAllBig;
			foreach (var tableInfo in tableInfos ?? [])
			{
				var rowCounts = IsSystemTable(tableInfo.Table) ? systemRowCounts : debugRowCounts;
				int colOffset = 0;
				foreach (var colInfo in tableInfo.Columns)
				{
					colInfo.Offset = colOffset;
					var colSize = GetSize(colInfo.ColumnType, rowCounts);
					colInfo.Size = colSize;
					colOffset += colSize;
				}
				tableInfo.RowSize = colOffset;
			}
		}

		int GetSize(ColumnType columnType, IList<uint> rowCounts)
		{
			if (ColumnType.Module <= columnType && columnType <= ColumnType.CustomDebugInformation)
			{
				int table = columnType - ColumnType.Module;
				uint count = table >= rowCounts.Count ? 0 : rowCounts[table];
				return forceAllBig || count > 0xFFFF ? 4 : 2;
			}
			else if (ColumnType.TypeDefOrRef <= columnType && columnType <= ColumnType.HasCustomDebugInformation)
			{
				var info = columnType switch
				{
					ColumnType.TypeDefOrRef => CodedToken.TypeDefOrRef,
					ColumnType.HasConstant => CodedToken.HasConstant,
					ColumnType.HasCustomAttribute => CodedToken.HasCustomAttribute,
					ColumnType.HasFieldMarshal => CodedToken.HasFieldMarshal,
					ColumnType.HasDeclSecurity => CodedToken.HasDeclSecurity,
					ColumnType.MemberRefParent => CodedToken.MemberRefParent,
					ColumnType.HasSemantic => CodedToken.HasSemantic,
					ColumnType.MethodDefOrRef => CodedToken.MethodDefOrRef,
					ColumnType.MemberForwarded => CodedToken.MemberForwarded,
					ColumnType.Implementation => CodedToken.Implementation,
					ColumnType.CustomAttributeType => CodedToken.CustomAttributeType,
					ColumnType.ResolutionScope => CodedToken.ResolutionScope,
					ColumnType.TypeOrMethodDef => CodedToken.TypeOrMethodDef,
					ColumnType.HasCustomDebugInformation => CodedToken.HasCustomDebugInformation,
					_ => throw new InvalidOperationException($"Invalid ColumnType: {columnType}"),
				};
				uint maxRows = 0;
				foreach (var tableType in info.TableTypes)
				{
					int index = (int)tableType;
					var tableRows = index >= rowCounts.Count ? 0 : rowCounts[index];
					if (tableRows > maxRows)
						maxRows = tableRows;
				}
				// Can't overflow since maxRows <= 0x00FFFFFF and info.Bits < 8
				uint finalRows = maxRows << info.Bits;
				return forceAllBig || finalRows > 0xFFFF ? 4 : 2;
			}
			else
			{
				switch (columnType)
				{
					case ColumnType.Byte: return 1;
					case ColumnType.Int16: return 2;
					case ColumnType.UInt16: return 2;
					case ColumnType.Int32: return 4;
					case ColumnType.UInt32: return 4;
					case ColumnType.Strings: return forceAllBig || bigStrings ? 4 : 2;
					case ColumnType.GUID: return forceAllBig || bigGuid ? 4 : 2;
					case ColumnType.Blob: return forceAllBig || bigBlob ? 4 : 2;
				}
			}
			throw new InvalidOperationException($"Invalid ColumnType: {columnType}");
		}

		public TableInfo[] CreateTables(byte majorVersion, byte minorVersion) =>
			CreateTables(majorVersion, minorVersion, out int maxPresentTables);

		internal const int normalMaxTables = (int)TableType.CustomDebugInformation + 1;

		public TableInfo[] CreateTables(byte majorVersion, byte minorVersion, out int maxPresentTables)
		{
			maxPresentTables = (majorVersion == 1 && minorVersion == 0) ? (int)TableType.NestedClass + 1 : normalMaxTables;

			var tableInfos = new TableInfo[normalMaxTables];

			tableInfos[(int)TableType.Module] = new TableInfo(TableType.Module, "Module", [
				new ColumnInfo(0, "Generation", ColumnType.UInt16),
				new ColumnInfo(1, "Name", ColumnType.Strings),
				new ColumnInfo(2, "Mvid", ColumnType.GUID),
				new ColumnInfo(3, "EncId", ColumnType.GUID),
				new ColumnInfo(4, "EncBaseId", ColumnType.GUID),
			]);
			tableInfos[(int)TableType.TypeRef] = new TableInfo(TableType.TypeRef, "TypeRef", [
				new ColumnInfo(0, "ResolutionScope", ColumnType.ResolutionScope),
				new ColumnInfo(1, "Name", ColumnType.Strings),
				new ColumnInfo(2, "Namespace", ColumnType.Strings),
			]);
			tableInfos[(int)TableType.TypeDef] = new TableInfo(TableType.TypeDef, "TypeDef", [
				new ColumnInfo(0, "Flags", ColumnType.UInt32),
				new ColumnInfo(1, "Name", ColumnType.Strings),
				new ColumnInfo(2, "Namespace", ColumnType.Strings),
				new ColumnInfo(3, "Extends", ColumnType.TypeDefOrRef),
				new ColumnInfo(4, "FieldList", ColumnType.Field),
				new ColumnInfo(5, "MethodList", ColumnType.Method),
			]);
			tableInfos[(int)TableType.FieldPtr] = new TableInfo(TableType.FieldPtr, "FieldPtr", [
				new ColumnInfo(0, "Field", ColumnType.Field),
			]);
			tableInfos[(int)TableType.Field] = new TableInfo(TableType.Field, "Field", [
				new ColumnInfo(0, "Flags", ColumnType.UInt16),
				new ColumnInfo(1, "Name", ColumnType.Strings),
				new ColumnInfo(2, "Signature", ColumnType.Blob),
			]);
			tableInfos[(int)TableType.MethodPtr] = new TableInfo(TableType.MethodPtr, "MethodPtr", [
				new ColumnInfo(0, "Method", ColumnType.Method),
			]);
			tableInfos[(int)TableType.Method] = new TableInfo(TableType.Method, "Method", [
				new ColumnInfo(0, "RVA", ColumnType.UInt32),
				new ColumnInfo(1, "ImplFlags", ColumnType.UInt16),
				new ColumnInfo(2, "Flags", ColumnType.UInt16),
				new ColumnInfo(3, "Name", ColumnType.Strings),
				new ColumnInfo(4, "Signature", ColumnType.Blob),
				new ColumnInfo(5, "ParamList", ColumnType.Param),
			]);
			tableInfos[(int)TableType.ParamPtr] = new TableInfo(TableType.ParamPtr, "ParamPtr", [
				new ColumnInfo(0, "Param", ColumnType.Param),
			]);
			tableInfos[(int)TableType.Param] = new TableInfo(TableType.Param, "Param", [
				new ColumnInfo(0, "Flags", ColumnType.UInt16),
				new ColumnInfo(1, "Sequence", ColumnType.UInt16),
				new ColumnInfo(2, "Name", ColumnType.Strings),
			]);
			tableInfos[(int)TableType.InterfaceImpl] = new TableInfo(TableType.InterfaceImpl, "InterfaceImpl", [
				new ColumnInfo(0, "Class", ColumnType.TypeDef),
				new ColumnInfo(1, "Interface", ColumnType.TypeDefOrRef),
			]);
			tableInfos[(int)TableType.MemberRef] = new TableInfo(TableType.MemberRef, "MemberRef", [
				new ColumnInfo(0, "Class", ColumnType.MemberRefParent),
				new ColumnInfo(1, "Name", ColumnType.Strings),
				new ColumnInfo(2, "Signature", ColumnType.Blob),
			]);
			tableInfos[(int)TableType.Constant] = new TableInfo(TableType.Constant, "Constant", [
				new ColumnInfo(0, "Type", ColumnType.Byte),
				new ColumnInfo(1, "Padding", ColumnType.Byte),
				new ColumnInfo(2, "Parent", ColumnType.HasConstant),
				new ColumnInfo(3, "Value", ColumnType.Blob),
			]);
			tableInfos[(int)TableType.CustomAttribute] = new TableInfo(TableType.CustomAttribute, "CustomAttribute", [
				new ColumnInfo(0, "Parent", ColumnType.HasCustomAttribute),
				new ColumnInfo(1, "Type", ColumnType.CustomAttributeType),
				new ColumnInfo(2, "Value", ColumnType.Blob),
			]);
			tableInfos[(int)TableType.FieldMarshal] = new TableInfo(TableType.FieldMarshal, "FieldMarshal", [
				new ColumnInfo(0, "Parent", ColumnType.HasFieldMarshal),
				new ColumnInfo(1, "NativeType", ColumnType.Blob),
			]);
			tableInfos[(int)TableType.DeclSecurity] = new TableInfo(TableType.DeclSecurity, "DeclSecurity", [
				new ColumnInfo(0, "Action", ColumnType.Int16),
				new ColumnInfo(1, "Parent", ColumnType.HasDeclSecurity),
				new ColumnInfo(2, "PermissionSet", ColumnType.Blob),
			]);
			tableInfos[(int)TableType.ClassLayout] = new TableInfo(TableType.ClassLayout, "ClassLayout", [
				new ColumnInfo(0, "PackingSize", ColumnType.UInt16),
				new ColumnInfo(1, "ClassSize", ColumnType.UInt32),
				new ColumnInfo(2, "Parent", ColumnType.TypeDef),
			]);
			tableInfos[(int)TableType.FieldLayout] = new TableInfo(TableType.FieldLayout, "FieldLayout", [
				new ColumnInfo(0, "OffSet", ColumnType.UInt32),
				new ColumnInfo(1, "Field", ColumnType.Field),
			]);
			tableInfos[(int)TableType.StandAloneSig] = new TableInfo(TableType.StandAloneSig, "StandAloneSig", [
				new ColumnInfo(0, "Signature", ColumnType.Blob),
			]);
			tableInfos[(int)TableType.EventMap] = new TableInfo(TableType.EventMap, "EventMap", [
				new ColumnInfo(0, "Parent", ColumnType.TypeDef),
				new ColumnInfo(1, "EventList", ColumnType.Event),
			]);
			tableInfos[(int)TableType.EventPtr] = new TableInfo(TableType.EventPtr, "EventPtr", [
				new ColumnInfo(0, "Event", ColumnType.Event),
			]);
			tableInfos[(int)TableType.Event] = new TableInfo(TableType.Event, "Event", [
				new ColumnInfo(0, "EventFlags", ColumnType.UInt16),
				new ColumnInfo(1, "Name", ColumnType.Strings),
				new ColumnInfo(2, "EventType", ColumnType.TypeDefOrRef),
			]);
			tableInfos[(int)TableType.PropertyMap] = new TableInfo(TableType.PropertyMap, "PropertyMap", [
				new ColumnInfo(0, "Parent", ColumnType.TypeDef),
				new ColumnInfo(1, "PropertyList", ColumnType.Property),
			]);
			tableInfos[(int)TableType.PropertyPtr] = new TableInfo(TableType.PropertyPtr, "PropertyPtr", [
				new ColumnInfo(0, "Property", ColumnType.Property),
			]);
			tableInfos[(int)TableType.Property] = new TableInfo(TableType.Property, "Property", [
				new ColumnInfo(0, "PropFlags", ColumnType.UInt16),
				new ColumnInfo(1, "Name", ColumnType.Strings),
				new ColumnInfo(2, "Type", ColumnType.Blob),
			]);
			tableInfos[(int)TableType.MethodSemantics] = new TableInfo(TableType.MethodSemantics, "MethodSemantics", [
				new ColumnInfo(0, "Semantic", ColumnType.UInt16),
				new ColumnInfo(1, "Method", ColumnType.Method),
				new ColumnInfo(2, "Association", ColumnType.HasSemantic),
			]);
			tableInfos[(int)TableType.MethodImpl] = new TableInfo(TableType.MethodImpl, "MethodImpl", [
				new ColumnInfo(0, "Class", ColumnType.TypeDef),
				new ColumnInfo(1, "MethodBody", ColumnType.MethodDefOrRef),
				new ColumnInfo(2, "MethodDeclaration", ColumnType.MethodDefOrRef),
			]);
			tableInfos[(int)TableType.ModuleRef] = new TableInfo(TableType.ModuleRef, "ModuleRef", [
				new ColumnInfo(0, "Name", ColumnType.Strings),
			]);
			tableInfos[(int)TableType.TypeSpec] = new TableInfo(TableType.TypeSpec, "TypeSpec", [
				new ColumnInfo(0, "Signature", ColumnType.Blob),
			]);
			tableInfos[(int)TableType.ImplMap] = new TableInfo(TableType.ImplMap, "ImplMap", [
				new ColumnInfo(0, "MappingFlags", ColumnType.UInt16),
				new ColumnInfo(1, "MemberForwarded", ColumnType.MemberForwarded),
				new ColumnInfo(2, "ImportName", ColumnType.Strings),
				new ColumnInfo(3, "ImportScope", ColumnType.ModuleRef),
			]);
			tableInfos[(int)TableType.FieldRVA] = new TableInfo(TableType.FieldRVA, "FieldRVA", [
				new ColumnInfo(0, "RVA", ColumnType.UInt32),
				new ColumnInfo(1, "Field", ColumnType.Field),
			]);
			tableInfos[(int)TableType.ENCLog] = new TableInfo(TableType.ENCLog, "ENCLog", [
				new ColumnInfo(0, "Token", ColumnType.UInt32),
				new ColumnInfo(1, "FuncCode", ColumnType.UInt32),
			]);
			tableInfos[(int)TableType.ENCMap] = new TableInfo(TableType.ENCMap, "ENCMap", [
				new ColumnInfo(0, "Token", ColumnType.UInt32),
			]);
			tableInfos[(int)TableType.Assembly] = new TableInfo(TableType.Assembly, "Assembly", [
				new ColumnInfo(0, "HashAlgId", ColumnType.UInt32),
				new ColumnInfo(1, "MajorVersion", ColumnType.UInt16),
				new ColumnInfo(2, "MinorVersion", ColumnType.UInt16),
				new ColumnInfo(3, "BuildNumber", ColumnType.UInt16),
				new ColumnInfo(4, "RevisionNumber", ColumnType.UInt16),
				new ColumnInfo(5, "Flags", ColumnType.UInt32),
				new ColumnInfo(6, "PublicKey", ColumnType.Blob),
				new ColumnInfo(7, "Name", ColumnType.Strings),
				new ColumnInfo(8, "Locale", ColumnType.Strings),
			]);
			tableInfos[(int)TableType.AssemblyProcessor] = new TableInfo(TableType.AssemblyProcessor, "AssemblyProcessor", [
				new ColumnInfo(0, "Processor", ColumnType.UInt32),
			]);
			tableInfos[(int)TableType.AssemblyOS] = new TableInfo(TableType.AssemblyOS, "AssemblyOS", [
				new ColumnInfo(0, "OSPlatformId", ColumnType.UInt32),
				new ColumnInfo(1, "OSMajorVersion", ColumnType.UInt32),
				new ColumnInfo(2, "OSMinorVersion", ColumnType.UInt32),
			]);
			tableInfos[(int)TableType.AssemblyRef] = new TableInfo(TableType.AssemblyRef, "AssemblyRef", [
				new ColumnInfo(0, "MajorVersion", ColumnType.UInt16),
				new ColumnInfo(1, "MinorVersion", ColumnType.UInt16),
				new ColumnInfo(2, "BuildNumber", ColumnType.UInt16),
				new ColumnInfo(3, "RevisionNumber", ColumnType.UInt16),
				new ColumnInfo(4, "Flags", ColumnType.UInt32),
				new ColumnInfo(5, "PublicKeyOrToken", ColumnType.Blob),
				new ColumnInfo(6, "Name", ColumnType.Strings),
				new ColumnInfo(7, "Locale", ColumnType.Strings),
				new ColumnInfo(8, "HashValue", ColumnType.Blob),
			]);
			tableInfos[(int)TableType.AssemblyRefProcessor] = new TableInfo(TableType.AssemblyRefProcessor, "AssemblyRefProcessor", [
				new ColumnInfo(0, "Processor", ColumnType.UInt32),
				new ColumnInfo(1, "AssemblyRef", ColumnType.AssemblyRef),
			]);
			tableInfos[(int)TableType.AssemblyRefOS] = new TableInfo(TableType.AssemblyRefOS, "AssemblyRefOS", [
				new ColumnInfo(0, "OSPlatformId", ColumnType.UInt32),
				new ColumnInfo(1, "OSMajorVersion", ColumnType.UInt32),
				new ColumnInfo(2, "OSMinorVersion", ColumnType.UInt32),
				new ColumnInfo(3, "AssemblyRef", ColumnType.AssemblyRef),
			]);
			tableInfos[(int)TableType.File] = new TableInfo(TableType.File, "File", [
				new ColumnInfo(0, "Flags", ColumnType.UInt32),
				new ColumnInfo(1, "Name", ColumnType.Strings),
				new ColumnInfo(2, "HashValue", ColumnType.Blob),
			]);
			tableInfos[(int)TableType.ExportedType] = new TableInfo(TableType.ExportedType, "ExportedType", [
				new ColumnInfo(0, "Flags", ColumnType.UInt32),
				new ColumnInfo(1, "TypeDefId", ColumnType.UInt32),
				new ColumnInfo(2, "TypeName", ColumnType.Strings),
				new ColumnInfo(3, "TypeNamespace", ColumnType.Strings),
				new ColumnInfo(4, "Implementation", ColumnType.Implementation),
			]);
			tableInfos[(int)TableType.ManifestResource] = new TableInfo(TableType.ManifestResource, "ManifestResource", [
				new ColumnInfo(0, "Offset", ColumnType.UInt32),
				new ColumnInfo(1, "Flags", ColumnType.UInt32),
				new ColumnInfo(2, "Name", ColumnType.Strings),
				new ColumnInfo(3, "Implementation", ColumnType.Implementation),
			]);
			tableInfos[(int)TableType.NestedClass] = new TableInfo(TableType.NestedClass, "NestedClass", [
				new ColumnInfo(0, "NestedClass", ColumnType.TypeDef),
				new ColumnInfo(1, "EnclosingClass", ColumnType.TypeDef),
			]);
			if (majorVersion == 1 && minorVersion == 1)
			{
				tableInfos[(int)TableType.GenericParam] = new TableInfo(TableType.GenericParam, "GenericParam", [
					new ColumnInfo(0, "Number", ColumnType.UInt16),
					new ColumnInfo(1, "Flags", ColumnType.UInt16),
					new ColumnInfo(2, "Owner", ColumnType.TypeOrMethodDef),
					new ColumnInfo(3, "Name", ColumnType.Strings),
					new ColumnInfo(4, "Kind", ColumnType.TypeDefOrRef),
				]);
			}
			else
			{
				tableInfos[(int)TableType.GenericParam] = new TableInfo(TableType.GenericParam, "GenericParam", [
					new ColumnInfo(0, "Number", ColumnType.UInt16),
					new ColumnInfo(1, "Flags", ColumnType.UInt16),
					new ColumnInfo(2, "Owner", ColumnType.TypeOrMethodDef),
					new ColumnInfo(3, "Name", ColumnType.Strings),
				]);
			}
			tableInfos[(int)TableType.MethodSpec] = new TableInfo(TableType.MethodSpec, "MethodSpec", [
				new ColumnInfo(0, "Method", ColumnType.MethodDefOrRef),
				new ColumnInfo(1, "Instantiation", ColumnType.Blob),
			]);
			tableInfos[(int)TableType.GenericParamConstraint] = new TableInfo(TableType.GenericParamConstraint, "GenericParamConstraint", [
				new ColumnInfo(0, "Owner", ColumnType.GenericParam),
				new ColumnInfo(1, "Constraint", ColumnType.TypeDefOrRef),
			]);
			tableInfos[0x2D] = new TableInfo((TableType)0x2D, string.Empty, []);
			tableInfos[0x2E] = new TableInfo((TableType)0x2E, string.Empty, []);
			tableInfos[0x2F] = new TableInfo((TableType)0x2F, string.Empty, []);
			tableInfos[(int)TableType.Document] = new TableInfo(TableType.Document, "Document", [
				new ColumnInfo(0, "Name", ColumnType.Blob),
				new ColumnInfo(1, "HashAlgorithm", ColumnType.GUID),
				new ColumnInfo(2, "Hash", ColumnType.Blob),
				new ColumnInfo(3, "Language", ColumnType.GUID),
			]);
			tableInfos[(int)TableType.MethodDebugInformation] = new TableInfo(TableType.MethodDebugInformation, "MethodDebugInformation", [
				new ColumnInfo(0, "Document", ColumnType.Document),
				new ColumnInfo(1, "SequencePoints", ColumnType.Blob),
			]);
			tableInfos[(int)TableType.LocalScope] = new TableInfo(TableType.LocalScope, "LocalScope", [
				new ColumnInfo(0, "Method", ColumnType.Method),
				new ColumnInfo(1, "ImportScope", ColumnType.ImportScope),
				new ColumnInfo(2, "VariableList", ColumnType.LocalVariable),
				new ColumnInfo(3, "ConstantList", ColumnType.LocalConstant),
				new ColumnInfo(4, "StartOffset", ColumnType.UInt32),
				new ColumnInfo(5, "Length", ColumnType.UInt32),
			]);
			tableInfos[(int)TableType.LocalVariable] = new TableInfo(TableType.LocalVariable, "LocalVariable", [
				new ColumnInfo(0, "Attributes", ColumnType.UInt16),
				new ColumnInfo(1, "Index", ColumnType.UInt16),
				new ColumnInfo(2, "Name", ColumnType.Strings),
			]);
			tableInfos[(int)TableType.LocalConstant] = new TableInfo(TableType.LocalConstant, "LocalConstant", [
				new ColumnInfo(0, "Name", ColumnType.Strings),
				new ColumnInfo(1, "Signature", ColumnType.Blob),
			]);
			tableInfos[(int)TableType.ImportScope] = new TableInfo(TableType.ImportScope, "ImportScope", [
				new ColumnInfo(0, "Parent", ColumnType.ImportScope),
				new ColumnInfo(1, "Imports", ColumnType.Blob),
			]);
			tableInfos[(int)TableType.StateMachineMethod] = new TableInfo(TableType.StateMachineMethod, "StateMachineMethod", [
				new ColumnInfo(0, "MoveNextMethod", ColumnType.Method),
				new ColumnInfo(1, "KickoffMethod", ColumnType.Method),
			]);
			tableInfos[(int)TableType.CustomDebugInformation] = new TableInfo(TableType.CustomDebugInformation, "CustomDebugInformation", [
				new ColumnInfo(0, "Parent", ColumnType.HasCustomDebugInformation),
				new ColumnInfo(1, "Kind", ColumnType.GUID),
				new ColumnInfo(2, "Value", ColumnType.Blob),
			]);
			return this.tableInfos = tableInfos;
		}
	}
}