namespace ILSpy.Meta
{
    public interface IRowReader<TRow> where TRow : struct
    {
        bool TryReadRow(uint rid, out TRow row);
    }
}