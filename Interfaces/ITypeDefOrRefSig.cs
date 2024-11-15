namespace ILSpy.Interfaces
{
    public interface ITypeDefOrRefSig : IType
    {
        ITypeDefOrRef TypeDefOrRef { get; }
        bool IsTypeRef { get; }
    }
}