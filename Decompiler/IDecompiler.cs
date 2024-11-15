namespace ILSpy.Decompiler
{
    public interface IDecompiler
    {
        string UniqueNameUI { get; }
        string GenericNameUI { get; }
        Guid UniqueGuid { get; }
        Guid GenericGuid { get; }
    }
}