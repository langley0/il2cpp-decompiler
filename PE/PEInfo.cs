using ILSpy.IO;

namespace ILSpy.PE
{
    public class PEInfo
    {
        public ImageDosHeader ImageDosHeader { get; private set; }

        /// <summary>
        /// Returns the NT headers
        /// </summary>
        public ImageNTHeaders ImageNTHeaders { get; private set; }

        /// <summary>
        /// Returns the section headers
        /// </summary>
        public ImageSectionHeader[] ImageSectionHeaders { get; private set; }

        public PEInfo(DataReader reader)
        {
            reader.Position = 0;
            ImageDosHeader = new ImageDosHeader(reader);

            if (ImageDosHeader.NTHeadersOffset == 0)
            {
                throw new BadImageFormatException("Invalid NT headers offset");
            }
            reader.Position = ImageDosHeader.NTHeadersOffset;
            ImageNTHeaders = new ImageNTHeaders(reader);
            int numSections = ImageNTHeaders.FileHeader.NumberOfSections;
            ImageSectionHeaders = new ImageSectionHeader[numSections];
            for (int i = 0; i < ImageSectionHeaders.Length; i++)
            {
                ImageSectionHeaders[i] = new ImageSectionHeader(reader);
            }
        }

        public ImageSectionHeader? ToImageSectionHeader(FileOffset offset)
        {
            foreach (var section in ImageSectionHeaders)
            {
                if ((uint)offset >= section.PointerToRawData && (uint)offset < section.PointerToRawData + section.SizeOfRawData)
                {
                    return section;
                }
            }
            return null;
        }

        public ImageSectionHeader? ToImageSectionHeader(RVA rva)
        {
            uint alignment = ImageNTHeaders.OptionalHeader.SectionAlignment;
            foreach (var section in ImageSectionHeaders)
            {
                if (rva >= section.VirtualAddress && rva < section.VirtualAddress + AlignUp(section.VirtualSize, alignment))
                {
                    return section;
                }
            }
            return null;
        }

        public RVA ToRVA(FileOffset offset)
        {
            // In pe headers
            if (ImageSectionHeaders.Length == 0)
            {
                return (RVA)offset;
            }

            // In pe additional data, like digital signature, won't be loaded into memory
            var lastSection = ImageSectionHeaders[ImageSectionHeaders.Length - 1];
            if ((uint)offset > lastSection.PointerToRawData + lastSection.SizeOfRawData)
            {
                return 0;
            }

            // In a section
            var section = ToImageSectionHeader(offset);
            if (section is not null)
            {
                return (uint)(offset - section.PointerToRawData) + section.VirtualAddress;
            }

            // In pe headers
            return (RVA)offset;
        }

        public FileOffset ToFileOffset(RVA rva)
        {
            // Check if rva is larger than memory layout size
            if ((uint)rva >= ImageNTHeaders.OptionalHeader.SizeOfImage)
                return 0;

            var section = ToImageSectionHeader(rva);
            if (section is not null)
            {
                uint offset = rva - section.VirtualAddress;
                // Virtual size may be bigger than raw size and there may be no corresponding file offset to rva
                if (offset < section.SizeOfRawData)
                {
                    return (FileOffset)offset + section.PointerToRawData;
                }
                return 0;
            }

            // If not in any section, rva is in pe headers and don't convert it
            return (FileOffset)rva;
        }

        static uint AlignUp(uint val, uint alignment) => (val + alignment - 1) & ~(alignment - 1);
    }
}