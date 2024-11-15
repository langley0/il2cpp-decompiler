namespace ILSpy.Interfaces
{
    public interface IMemberRef : ICodedToken, IFullName, IOwnerModule, IIsTypeOrMethod
    {
        ITypeDefOrRef DeclaringType { get; }

        bool IsField { get; }

        bool IsTypeSpec { get; }

        bool IsTypeRef { get; }

        bool IsTypeDef { get; }

        bool IsMethodSpec { get; }

        bool IsMethodDef { get; }

        bool IsMemberRef { get; }

        bool IsFieldDef { get; }

        bool IsPropertyDef { get; }

        bool IsEventDef { get; }

        bool IsGenericParam { get; }
    }
}