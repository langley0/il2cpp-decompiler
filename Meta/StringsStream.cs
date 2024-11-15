using ILSpy.IO;

namespace ILSpy.Meta
{
    public sealed class StringsStream : HeapStream
    {
        public StringsStream()
        {
        }

        public StringsStream(DataReaderFactory mdReaderFactory, uint metadataBaseOffset, StreamHeader streamHeader)
            : base(mdReaderFactory, metadataBaseOffset, streamHeader)
        {
        }

        public UTF8String? Read(uint offset)
        {
            if (offset >= StreamLength)
            {
                return null;
            }

            byte[]? data;
            var reader = dataReader;
            if (reader is null)
            {
                return null;
            }

            reader.Position = offset;
            data = reader.TryReadBytesUntil(0);
            if (data is null)
            {
                return null;
            }
            return new UTF8String(data);
        }
        public UTF8String ReadNoNull(uint offset) => Read(offset) ?? UTF8String.Empty;
    }
}