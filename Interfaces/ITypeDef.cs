namespace ILSpy.Interfaces
{
    public interface ITypeDef : ITypeDefOrRef, IHasCustomAttribute, IHasDeclSecurity, IMemberRefParent, ITypeOrMethodDef, IHasCustomDebugInformation, IListListener<IFieldDef>, IListListener<IMethodDef>, IListListener<ITypeDef>, IListListener<IEventDef>, IListListener<IPropertyDef>, IListListener<IGenericParameter>, IMemberRefResolver, IMemberDef
    {
    }
}