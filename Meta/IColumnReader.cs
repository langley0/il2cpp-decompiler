namespace ILSpy.Meta
{
    public interface IColumnReader
    {
        bool ReadColumn(Table table, uint rid, ColumnInfo column, out uint value);
    }
}