namespace ILSpy.PE
{
    public enum Subsystem : ushort
    {
        Unknown = 0,
        Native = 1,
        WindowsGui = 2,
        WindowsCui = 3,
        Os2Cui = 5,
        PosixCui = 7,
        NativeWindows = 8,
        WindowsCeGui = 9,
        EfiApplication = 10,
        EfiBootServiceDriver = 11,
        EfiRuntimeDriver = 12,
        EfiRom = 13,
        Xbox = 14,
        WindowsBootApplication = 16,
    }

}