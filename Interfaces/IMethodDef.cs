namespace ILSpy.Interfaces
{
    public interface IMethodDef : IHasCustomAttribute, IHasDeclSecurity, IMemberRefParent, IMethodDefOrRef, IMemberForwarded, ICustomAttributeType, ITypeOrMethodDef, IManagedEntryPoint, IHasCustomDebugInformation, IListListener<IGenericParameter>, IListListener<IParamDef>, IMemberDef
    {
    }
}