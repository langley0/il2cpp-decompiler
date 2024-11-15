namespace ILSpy.Interfaces
{
    public interface ITypeRef : ITypeDefOrRef, IHasCustomAttribute, IMemberRefParent, IHasCustomDebugInformation, IResolutionScope
    {
    }
}