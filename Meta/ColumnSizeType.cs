namespace ILSpy.Meta
{
    public enum ColumnType : byte
    {
        Module,
        TypeRef,
        TypeDef,
        FieldPtr,
        Field,
        MethodPtr,
        Method,
        ParamPtr,
        Param,
        InterfaceImpl,
        MemberRef,
        Constant,
        CustomAttribute,
        FieldMarshal,
        DeclSecurity,
        ClassLayout,
        FieldLayout,
        StandAloneSig,
        EventMap,
        EventPtr,
        Event,
        PropertyMap,
        PropertyPtr,
        Property,
        MethodSemantics,
        MethodImpl,
        ModuleRef,
        TypeSpec,
        ImplMap,
        FieldRVA,
        ENCLog,
        ENCMap,
        Assembly,
        AssemblyProcessor,
        AssemblyOS,
        AssemblyRef,
        AssemblyRefProcessor,
        AssemblyRefOS,
        File,
        ExportedType,
        ManifestResource,
        NestedClass,
        GenericParam,
        MethodSpec,
        GenericParamConstraint,
        Document = 0x30,
        MethodDebugInformation,
        LocalScope,
        LocalVariable,
        LocalConstant,
        ImportScope,
        StateMachineMethod,
        CustomDebugInformation,
        Byte = 0x40,
        Int16,
        UInt16,
        Int32,
        UInt32,
        Strings,
        GUID,
        Blob,
        TypeDefOrRef,
        HasConstant,
        HasCustomAttribute,
        HasFieldMarshal,
        HasDeclSecurity,
        MemberRefParent,
        HasSemantic,
        MethodDefOrRef,
        MemberForwarded,
        Implementation,
        CustomAttributeType,
        ResolutionScope,
        TypeOrMethodDef,
        HasCustomDebugInformation,
    }
}