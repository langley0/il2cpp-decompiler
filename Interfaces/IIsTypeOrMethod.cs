namespace ILSpy.Interfaces
{
    public interface IIsTypeOrMethod
    {
        bool IsType { get; }

        bool IsMethod { get; }
    }
}