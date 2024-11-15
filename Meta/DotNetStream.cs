using ILSpy.IO;

namespace ILSpy.Meta
{
    public abstract class DotNetStream : IFileSection, IDisposable
    {
        protected DataReader? dataReader;

        StreamHeader? streamHeader;

        DataReaderFactory? mdReaderFactory;
        readonly uint metadataBaseOffset;

        public FileOffset StartOffset => (FileOffset)(dataReader?.StartOffset ?? 0);

        public FileOffset EndOffset => (FileOffset)(dataReader?.EndOffset ?? 0);

        public uint StreamLength => dataReader?.Length ?? 0;

        public StreamHeader? StreamHeader => streamHeader;

        public string Name => streamHeader is null ? string.Empty : streamHeader.Name;

        public DataReader CreateReader()
        {
            if (dataReader is null)
            {
                throw new Exception("No reader available");
            }
            return dataReader;
        }

        protected DotNetStream()
        {
            streamHeader = default;
            dataReader = default;
            mdReaderFactory = default;
        }

        protected DotNetStream(DataReaderFactory mdReaderFactory, uint metadataBaseOffset, StreamHeader streamHeader)
        {
            this.mdReaderFactory = mdReaderFactory;
            mdReaderFactory.DataReaderInvalidated += DataReaderFactory_DataReaderInvalidated;
            this.mdReaderFactory = mdReaderFactory;
            this.metadataBaseOffset = metadataBaseOffset;
            this.streamHeader = streamHeader;
            RecreateReader(mdReaderFactory, metadataBaseOffset, streamHeader, notifyThisClass: false);
        }

        void DataReaderFactory_DataReaderInvalidated(object? sender, EventArgs e) => RecreateReader(mdReaderFactory, metadataBaseOffset, streamHeader, notifyThisClass: true);

        void RecreateReader(DataReaderFactory? mdReaderFactory, uint metadataBaseOffset, StreamHeader? streamHeader, bool notifyThisClass)
        {
            if (mdReaderFactory is null || streamHeader is null)
            {
                dataReader = default;
            }
            else
            {
                dataReader = mdReaderFactory.CreateReader(metadataBaseOffset + streamHeader.Offset, streamHeader.StreamSize);
            }
            if (notifyThisClass)
            {
                OnReaderRecreated();
            }
        }

        protected virtual void OnReaderRecreated() { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var mdReaderFactory = this.mdReaderFactory;
                if (mdReaderFactory is not null)
                {
                    mdReaderFactory.DataReaderInvalidated -= DataReaderFactory_DataReaderInvalidated;
                }
                streamHeader = null;
                this.mdReaderFactory = null;
            }
        }

        public virtual bool IsValidIndex(uint index) => IsValidOffset(index);

        public bool IsValidOffset(uint offset) => offset == 0 || offset < dataReader?.Length;

        public bool IsValidOffset(uint offset, int size)
        {
            if (size == 0)
            {
                return IsValidOffset(offset);
            }
            return size > 0 && (ulong)offset + (uint)size <= dataReader?.Length;
        }
    }
}