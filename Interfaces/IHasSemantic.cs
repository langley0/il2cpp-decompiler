namespace ILSpy.Interfaces
{
    public interface IHasSemantic : ICodedToken, IHasCustomAttribute, IFullName, IMemberRef
    {
        int HasSemanticTag { get; }
    }
}