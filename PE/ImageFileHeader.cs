using ILSpy.IO;

namespace ILSpy.PE
{
    public sealed class ImageFileHeader : FileSection
    {
        public Machine Machine { get; private set; }
        public int NumberOfSections { get; private set; }
        public uint TimeDateStamp { get; private set; }
        public uint PointerToSymbolTable { get; private set; }
        public uint NumberOfSymbols { get; private set; }
        public uint SizeOfOptionalHeader { get; private set; }
        public Characteristics Characteristics { get; private set; }

        public ImageFileHeader(DataReader reader)
        {
            SetStartOffset(reader.CurrentOffset);
            Machine = (Machine)reader.ReadUInt16();
            NumberOfSections = reader.ReadUInt16();
            TimeDateStamp = reader.ReadUInt32();
            PointerToSymbolTable = reader.ReadUInt32();
            NumberOfSymbols = reader.ReadUInt32();
            SizeOfOptionalHeader = reader.ReadUInt16();
            Characteristics = (Characteristics)reader.ReadUInt16();
            SetEndOffset(reader.CurrentOffset);

            if (SizeOfOptionalHeader == 0) {
                throw new BadImageFormatException("Invalid SizeOfOptionalHeader");
            }
        }
    }
}