using ILSpy.IO;

namespace ILSpy.PE {
    public sealed class ImageNTHeaders : FileSection {
        public uint Signature { get; private set;}
        public ImageFileHeader FileHeader { get; private set;}
		public IImageOptionalHeader OptionalHeader{ get; private set;}

        public ImageNTHeaders(DataReader reader) {
			SetStartOffset(reader.CurrentOffset);
			Signature = reader.ReadUInt32();
			// Mono only checks the low 2 bytes
			if ((ushort)Signature != 0x4550) {
				throw new BadImageFormatException("Invalid NT headers signature");
            }
			FileHeader = new ImageFileHeader(reader);
			OptionalHeader = CreateImageOptionalHeader(reader);
			SetEndOffset(reader.CurrentOffset);
		}

        IImageOptionalHeader CreateImageOptionalHeader(DataReader reader) {
			ushort magic = reader.ReadUInt16();
			reader.Position -= 2;
			return magic switch {
				0x010B => new ImageOptionalHeader32(reader, FileHeader.SizeOfOptionalHeader),
				0x020B => new ImageOptionalHeader64(reader, FileHeader.SizeOfOptionalHeader),
				_ => throw new BadImageFormatException("Invalid optional header magic"),
			};
		}
    }
}
