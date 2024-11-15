using System.Text;
using ILSpy.IO;

namespace ILSpy.PE
{
    public sealed class ImageSectionHeader : FileSection
    {
        public string DisplayName { get; private set; }
        public byte[] Name { get; private set; }
        public uint VirtualSize { get; private set; }
        public RVA VirtualAddress { get; private set; }
        public uint SizeOfRawData { get; private set; }
        public uint PointerToRawData { get; private set; }
        public uint PointerToRelocations { get; private set; }
        public uint PointerToLinenumbers { get; private set; }
        public ushort NumberOfRelocations { get; private set; }
        public ushort NumberOfLinenumbers { get; private set; }
        public uint Characteristics { get; private set; }

        public ImageSectionHeader(DataReader reader)
        {
            SetStartOffset(reader.CurrentOffset);
            Name = reader.ReadBytes(8);
            VirtualSize = reader.ReadUInt32();
            VirtualAddress = (RVA)reader.ReadUInt32();
            SizeOfRawData = reader.ReadUInt32();
            PointerToRawData = reader.ReadUInt32();
            PointerToRelocations = reader.ReadUInt32();
            PointerToLinenumbers = reader.ReadUInt32();
            NumberOfRelocations = reader.ReadUInt16();
            NumberOfLinenumbers = reader.ReadUInt16();
            Characteristics = reader.ReadUInt32();
            DisplayName = ToString(Name);
            SetEndOffset(reader.CurrentOffset);

        }

        string ToString(byte[] name)
        {
            var sb = new StringBuilder(name.Length);
            foreach (var b in name)
            {
                if (b == 0)
                    break;
                sb.Append((char)b);
            }
            return sb.ToString();
        }
    }
}