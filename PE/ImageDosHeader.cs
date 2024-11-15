using ILSpy.IO;

namespace ILSpy.PE
{
    public sealed class ImageDosHeader : FileSection {
        public uint NTHeadersOffset { get; private set;}

        public ImageDosHeader(DataReader reader) {
			SetStartOffset(reader.CurrentOffset);
			ushort sig = reader.ReadUInt16();
			if (sig != 0x5A4D) {
				throw new BadImageFormatException("Invalid DOS signature");
            }
			reader.Position = (uint)StartOffset + 0x3C;
			NTHeadersOffset = reader.ReadUInt32();
			SetEndOffset(reader.CurrentOffset);
		}
    }
}