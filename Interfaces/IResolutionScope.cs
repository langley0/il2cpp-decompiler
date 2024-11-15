namespace ILSpy.Interfaces
{
    public interface IResolutionScope : ICodedToken, IHasCustomAttribute, IFullName
    {
        int ResolutionScopeTag { get; }
    }
}