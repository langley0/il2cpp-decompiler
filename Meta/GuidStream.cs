using ILSpy.IO;

namespace ILSpy.Meta
{
    public sealed class GuidStream : HeapStream
    {
        /// <inheritdoc/>
        public GuidStream()
        {
        }

        public GuidStream(DataReaderFactory mdReaderFactory, uint metadataBaseOffset, StreamHeader streamHeader)
            : base(mdReaderFactory, metadataBaseOffset, streamHeader)
        {
        }

        public override bool IsValidIndex(uint index) => index == 0 || (index <= 0x10000000 && IsValidOffset((index - 1) * 16, 16));

        public Guid? Read(uint index)
        {
            if (index == 0 || !IsValidIndex(index))
            {
                return null;
            }
            var reader = dataReader;
            if (reader is null)
            {
                return null;
            }
            reader.Position = (index - 1) * 16;
            return reader.ReadGuid();
        }
    }
}