using ILSpy.IO;

namespace ILSpy.Meta
{
	public sealed class Table : IDisposable, IFileSection
	{
		readonly TableType tableType;
		uint numRows;
		TableInfo? tableInfo;
		DataReader? dataReader;

		public FileOffset StartOffset => (FileOffset)(dataReader?.StartOffset ?? 0);

		public FileOffset EndOffset => (FileOffset)(dataReader?.EndOffset ?? 0);

		public TableType TableType => tableType;

		public string Name => tableInfo?.Name ?? string.Empty;

		public uint Rows => numRows;

		public uint RowSize => (uint)(tableInfo?.RowSize ?? 0);

		public IList<ColumnInfo> Columns => tableInfo?.Columns ?? [];

		public bool IsEmpty => numRows == 0;

		public TableInfo? TableInfo => tableInfo;

		internal DataReader? DataReader
		{
			get => dataReader;
			set => dataReader = value;
		}

		internal Table(TableType tableType, uint numRows, TableInfo tableInfo)
		{
			this.tableType = tableType;
			this.numRows = numRows;
			this.tableInfo = tableInfo;

			var columns = tableInfo.Columns;
			int length = columns.Length;
			if (length > 0) Column0 = columns[0];
			if (length > 1) Column1 = columns[1];
			if (length > 2) Column2 = columns[2];
			if (length > 3) Column3 = columns[3];
			if (length > 4) Column4 = columns[4];
			if (length > 5) Column5 = columns[5];
			if (length > 6) Column6 = columns[6];
			if (length > 7) Column7 = columns[7];
			if (length > 8) Column8 = columns[8];
		}

		// So we don't have to call IList<T> indexer
		internal readonly ColumnInfo? Column0;
		internal readonly ColumnInfo? Column1;
		internal readonly ColumnInfo? Column2;
		internal readonly ColumnInfo? Column3;
		internal readonly ColumnInfo? Column4;
		internal readonly ColumnInfo? Column5;
		internal readonly ColumnInfo? Column6;
		internal readonly ColumnInfo? Column7;
		internal readonly ColumnInfo? Column8;

		public bool IsValidRID(uint rid) => rid != 0 && rid <= numRows;

		public bool IsInvalidRID(uint rid) => rid == 0 || rid > numRows;

		/// <inheritdoc/>
		public void Dispose()
		{
			numRows = 0;
			tableInfo = null;
			dataReader = default;
		}
	}
}