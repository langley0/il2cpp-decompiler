namespace ILSpy.PE
{
    [Flags]
    public enum DllCharacteristics : ushort
    {
        Reserved1 = 0x0001,
        Reserved2 = 0x0002,
        Reserved3 = 0x0004,
        Reserved4 = 0x0008,
        Reserved5 = 0x0010,
        HighEntropyVA = 0x0020,
        DynamicBase = 0x0040,
        ForceIntegrity = 0x0080,
        NxCompat = 0x0100,
        NoIsolation = 0x0200,
        NoSeh = 0x0400,
        NoBind = 0x0800,
        AppContainer = 0x1000,
        WdmDriver = 0x2000,
        GuardCf = 0x4000,
        TerminalServerAware = 0x8000,
    }
}