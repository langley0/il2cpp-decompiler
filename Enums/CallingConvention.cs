namespace ILSpy.Enums
{
    [Flags]
    public enum CallingConvention : byte
    {
        Default = 0x0,
        C = 0x1,
        StdCall = 0x2,
        ThisCall = 0x3,
        FastCall = 0x4,
        VarArg = 0x5,
        Field = 0x6,
        LocalSig = 0x7,
        Property = 0x8,
        Unmanaged = 0x9,
        GenericInst = 0xA,
        NativeVarArg = 0xB,

        Mask = 0x0F,

        Generic = 0x10,
        HasThis = 0x20,
        ExplicitThis = 0x40,
        ReservedByCLR = 0x80,
    }
}