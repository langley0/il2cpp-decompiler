namespace ILSpy.Interfaces
{
    public interface IFullName
    {
        string FullName { get; }

        UTF8String Name { get; set; }
    }
}