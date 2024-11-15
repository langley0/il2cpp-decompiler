namespace ILSpy.Interfaces
{
    public interface ICustomAttributeCollection
    {
        int Count { get; }
        ICustomAttribute this[int index] { get; }
    }
}