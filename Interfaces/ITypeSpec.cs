namespace ILSpy.Interfaces
{
    public interface ITypeSpec : ITypeDefOrRef, IHasCustomAttribute, IMemberRefParent, IHasCustomDebugInformation
    {
        ITypeSig TypeSig { get; }
    }
}