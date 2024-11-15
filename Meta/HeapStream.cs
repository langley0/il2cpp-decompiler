using ILSpy.IO;

namespace ILSpy.Meta
{
    public abstract class HeapStream : DotNetStream
    {
        /// <inheritdoc/>
        protected HeapStream()
        {
        }

        /// <inheritdoc/>
        protected HeapStream(DataReaderFactory mdReaderFactory, uint metadataBaseOffset, StreamHeader streamHeader)
            : base(mdReaderFactory, metadataBaseOffset, streamHeader)
        {
        }
    }
}