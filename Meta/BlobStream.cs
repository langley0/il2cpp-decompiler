using ILSpy.IO;

namespace ILSpy.Meta
{
    public sealed class BlobStream : HeapStream
    {
        public BlobStream()
        {
        }

        public BlobStream(DataReaderFactory mdReaderFactory, uint metadataBaseOffset, StreamHeader streamHeader)
            : base(mdReaderFactory, metadataBaseOffset, streamHeader)
        {
        }

        public byte[]? Read(uint offset)
        {
            // The CLR has a special check for offset 0. It always interprets it as
            // 0-length data, even if that first byte isn't 0 at all.
            if (offset == 0)
            {
                return [];
            }
            if (!TryCreateReader(offset, out var reader))
            {
                return null;
            }
            return reader.ToArray();
        }

        public byte[] ReadNoNull(uint offset) => Read(offset) ?? [];

        public DataReader? CreateReader(uint offset)
        {
            if (TryCreateReader(offset, out var reader))
            {
                return reader;
            }
            return default;
        }

        public bool TryCreateReader(uint offset, out DataReader reader)
        {
            if (dataReader is null)
            {
                throw new Exception("No reader available");
            }

            reader = dataReader;
            if (!IsValidOffset(offset))
            {
                return false;
            }
            reader.Position = offset;
            if (!reader.TryReadCompressedUInt32(out uint length))
            {
                return false;
            }
            if (!reader.CanRead(length))
            {
                return false;
            }
            reader = reader.Slice(reader.Position, length);
            return true;
        }
    }
}
