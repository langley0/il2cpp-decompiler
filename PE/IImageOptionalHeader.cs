namespace ILSpy.PE
{
    public interface IImageOptionalHeader
    {
        public ushort Magic { get; }
        public byte MajorLinkerVersion { get; }
        public byte MinorLinkerVersion { get; }
        public uint SizeOfCode { get; }
        public uint SizeOfInitializedData { get; }
        public uint SizeOfUninitializedData { get; }
        public RVA AddressOfEntryPoint { get; }
        public RVA BaseOfCode { get; }
        public RVA BaseOfData { get; }
        public ulong ImageBase { get; }
        public uint SectionAlignment { get; }
        public uint FileAlignment { get; }
        public ushort MajorOperatingSystemVersion { get; }
        public ushort MinorOperatingSystemVersion { get; }
        public ushort MajorImageVersion { get; }
        public ushort MinorImageVersion { get; }
        public ushort MajorSubsystemVersion { get; }
        public ushort MinorSubsystemVersion { get; }
        public uint Win32VersionValue { get; }
        public uint SizeOfImage { get; }
        public uint SizeOfHeaders { get; }
        public uint CheckSum { get; }
        public Subsystem Subsystem { get; }
        public DllCharacteristics DllCharacteristics { get; }
        public ulong SizeOfStackReserve { get; }
        public ulong SizeOfStackCommit { get; }
        public ulong SizeOfHeapReserve { get; }
        public ulong SizeOfHeapCommit { get; }
        public uint LoaderFlags { get; }
        public uint NumberOfRvaAndSizes { get; }
        public ImageDataDirectory[] DataDirectories { get; }

    }
}