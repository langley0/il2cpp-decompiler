using ILSpy.IO;

namespace ILSpy.PE
{
    public interface IPEImage : IRvaFileOffsetConverter, IDisposable
    {
        string Filename { get; }
        ImageDosHeader ImageDosHeader { get; }
        ImageNTHeaders ImageNTHeaders { get; }
        IList<ImageSectionHeader> ImageSectionHeaders { get; }

        DataReader CreateReader(FileOffset offset, uint length);

        DataReader CreateReader(RVA rva, uint length);

        DataReader CreateReader(RVA rva);

        DataReaderFactory DataReaderFactory { get; }
    }
}