using ILSpy.IO;

namespace ILSpy.Meta
{
    public sealed class USStream : HeapStream
    {
        public USStream()
        {
        }

        public USStream(DataReaderFactory mdReaderFactory, uint metadataBaseOffset, StreamHeader streamHeader)
            : base(mdReaderFactory, metadataBaseOffset, streamHeader)
        {
        }

        public string? Read(uint offset)
        {
            if (offset == 0) { return string.Empty; }
            if (!IsValidOffset(offset)) { return null; }

            var reader = dataReader;
            if (reader is null) { return null; }

            reader.Position = offset;
            if (!reader.TryReadCompressedUInt32(out uint length)) { return null; }
            if (!reader.CanRead(length)) { return null; }

            try
            {
                return reader.ReadUtf16String((int)(length / 2));
            }
            catch (OutOfMemoryException)
            {
                throw;
            }
            catch
            {
                // It's possible that an exception is thrown when converting a char* to
                // a string. If so, return an empty string.
                return string.Empty;
            }
        }

        public string ReadNoNull(uint offset) => Read(offset) ?? string.Empty;
    }
}