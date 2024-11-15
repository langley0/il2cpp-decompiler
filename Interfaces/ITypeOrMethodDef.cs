namespace ILSpy.Interfaces
{
    public interface ITypeOrMethodDef : ICodedToken, IHasCustomAttribute, IHasDeclSecurity, IMemberRefParent, IFullName, IMemberRef, IGenericParameterProvider, IMemberDef
    {
        int TypeOrMethodDefTag { get; }

        IList<IGenericParameter> GenericParameters { get; }

        bool HasGenericParameters { get; }
    }
}