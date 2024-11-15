namespace ILSpy.Meta
{
    [Flags]
    public enum ComImageFlags : uint
    {
        ILOnly = 1,

        Bit32Required = 2,

        ILLibrary = 4,

        StrongNameSigned = 8,

        NativeEntryPoint = 0x10,

        TrackDebugData = 0x10000,

        Bit32Preferred = 0x20000,
    }
}