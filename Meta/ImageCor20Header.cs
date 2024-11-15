using ILSpy.IO;
using ILSpy.PE;

namespace ILSpy.Meta
{
    public sealed class ImageCor20Header : FileSection
    {
        readonly uint cb;
        readonly ushort majorRuntimeVersion;
        readonly ushort minorRuntimeVersion;
        readonly ImageDataDirectory metadata;
        readonly ComImageFlags flags;
        readonly uint entryPointToken_or_RVA;
        readonly ImageDataDirectory resources;
        readonly ImageDataDirectory strongNameSignature;
        readonly ImageDataDirectory codeManagerTable;
        readonly ImageDataDirectory vtableFixups;
        readonly ImageDataDirectory exportAddressTableJumps;
        readonly ImageDataDirectory managedNativeHeader;

        public bool HasNativeHeader => (flags & ComImageFlags.ILLibrary) != 0;

        public uint CB => cb;

        public ushort MajorRuntimeVersion => majorRuntimeVersion;

        public ushort MinorRuntimeVersion => minorRuntimeVersion;

        public ImageDataDirectory Metadata => metadata;

        public ComImageFlags Flags => flags;

        public uint EntryPointToken_or_RVA => entryPointToken_or_RVA;

        public ImageDataDirectory Resources => resources;

        public ImageDataDirectory StrongNameSignature => strongNameSignature;

        public ImageDataDirectory CodeManagerTable => codeManagerTable;

        public ImageDataDirectory VTableFixups => vtableFixups;

        public ImageDataDirectory ExportAddressTableJumps => exportAddressTableJumps;

        public ImageDataDirectory ManagedNativeHeader => managedNativeHeader;

        public ImageCor20Header(DataReader reader)
        {
            SetStartOffset(reader.CurrentOffset);
            cb = reader.ReadUInt32();
            if (cb < 0x48)
            {
                throw new BadImageFormatException("Invalid IMAGE_COR20_HEADER.cb value");
            }

            majorRuntimeVersion = reader.ReadUInt16();
            minorRuntimeVersion = reader.ReadUInt16();
            metadata = new ImageDataDirectory(reader);
            flags = (ComImageFlags)reader.ReadUInt32();
            entryPointToken_or_RVA = reader.ReadUInt32();
            resources = new ImageDataDirectory(reader);
            strongNameSignature = new ImageDataDirectory(reader);
            codeManagerTable = new ImageDataDirectory(reader);
            vtableFixups = new ImageDataDirectory(reader);
            exportAddressTableJumps = new ImageDataDirectory(reader);
            managedNativeHeader = new ImageDataDirectory(reader);
            SetEndOffset(reader.CurrentOffset);
        }
    }
}