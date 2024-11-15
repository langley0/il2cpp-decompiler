namespace ILSpy.Enums
{
    [Flags]
    public enum AssemblyAttributes : uint
    {
        None = 0,
        PublicKey = 1,
        PA_None = None,
        PA_MSIL = 0x0010,
        PA_x86 = 0x0020,
        PA_IA64 = 0x0030,
        PA_AMD64 = 0x0040,
        PA_ARM = 0x0050,
        PA_ARM64 = 0x0060,
        PA_NoPlatform = 0x0070,
        PA_Specified = 0x0080,

        PA_Mask = PA_NoPlatform,
        PA_FullMask = 0x00F0,
        PA_Shift = 0x0004,

        EnableJITcompileTracking = 0x8000,
        DisableJITcompileOptimizer = 0x4000,

        Retargetable = 0x0100,

        ContentType_Default = None,
        ContentType_WindowsRuntime = 0x0200,
        ContentType_Mask = 0x0E00,
    }
}