namespace ILSpy.Interfaces
{
    public interface ITypeResolver
    {
        ITypeDef? Resolve(ITypeRef typeRef, IModuleDef sourceModule);
    }
}