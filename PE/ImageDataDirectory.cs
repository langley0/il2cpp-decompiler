using ILSpy.IO;

namespace ILSpy.PE
{
    public sealed class ImageDataDirectory : FileSection
    {
        public RVA VirtualAddress { get; private set; }
        public uint Size { get; private set; }

        public ImageDataDirectory(DataReader reader)
        {
            SetStartOffset(reader.CurrentOffset);
            VirtualAddress = (RVA)reader.ReadUInt32();
            Size = reader.ReadUInt32();
            SetEndOffset(reader.CurrentOffset);
        }
        
        public ImageDataDirectory() {
            // This means "Empty Value"
        }

        

    }
}