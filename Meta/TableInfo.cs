namespace ILSpy.Meta
{
    public sealed class TableInfo {
		readonly TableType tableType;
		int rowSize;
		readonly ColumnInfo[] columns;
		readonly string name;

		public TableType Table => tableType;

		public int RowSize {
			get => rowSize;
			internal set => rowSize = value;
		}

		public ColumnInfo[] Columns => columns;

		public string Name => name;

		public TableInfo(TableType tableType, string name, ColumnInfo[] columns) {
			this.tableType = tableType;
			this.name = name;
			this.columns = columns;
		}

		public TableInfo(TableType tableType, string name, ColumnInfo[] columns, int rowSize) {
			this.tableType = tableType;
			this.name = name;
			this.columns = columns;
			this.rowSize = rowSize;
		}
	}
}