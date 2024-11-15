namespace ILSpy.Interfaces
{
    public interface IImplementation : ICodedToken, IHasCustomAttribute, IFullName
    {
        int ImplementationTag { get; }
    }
}