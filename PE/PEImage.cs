using ILSpy.IO;

namespace ILSpy.PE
{
    public sealed class PEImage : IPEImage
    {
        readonly DataReaderFactory dataReaderFactory;

        readonly PEInfo peInfo;

        string IPEImage.Filename => dataReaderFactory.Filename;

        ImageDosHeader IPEImage.ImageDosHeader => peInfo.ImageDosHeader;

        ImageNTHeaders IPEImage.ImageNTHeaders => peInfo.ImageNTHeaders;

        IList<ImageSectionHeader> IPEImage.ImageSectionHeaders => peInfo.ImageSectionHeaders;

        DataReaderFactory IPEImage.DataReaderFactory => dataReaderFactory;

        public PEImage(DataReaderFactory dataReaderFactory)
        {
            try
            {
                this.dataReaderFactory = dataReaderFactory;
                var reader = dataReaderFactory.CreateReader();
                peInfo = new PEInfo(reader);
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public PEImage(string filename)
            : this(DataReaderFactoryFactory.Create(filename))
        {
        }

        public PEImage(byte[] data)
            : this(data, "")
        {
        }

        public PEImage(byte[] data, string filename)
            : this(ByteArrayDataReaderFactory.Create(data, filename))
        {
        }

        public DataReader CreateReader(FileOffset offset)
        {
            return dataReaderFactory.CreateReader((uint)offset, dataReaderFactory.Length - (uint)offset);
        }

        public DataReader CreateReader(FileOffset offset, uint length)
        {
            return dataReaderFactory.CreateReader((uint)offset, length);
        }

        public DataReader CreateReader(RVA rva)
        {
            return CreateReader(ToFileOffset(rva));
        }

        public DataReader CreateReader(RVA rva, uint length)
        {
            return CreateReader(ToFileOffset(rva), length);
        }

        public RVA ToRVA(FileOffset offset) => peInfo.ToRVA(offset);

        public FileOffset ToFileOffset(RVA rva) => peInfo.ToFileOffset(rva);


        public void Dispose()
        {
            dataReaderFactory.Dispose();
        }
    }
}