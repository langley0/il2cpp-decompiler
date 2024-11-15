namespace ILSpy.IO
{
    public abstract class DataReaderFactory : IDisposable
    {
        public abstract string Filename { get; }

        public abstract uint Length { get; }

        public abstract DataReader CreateReader(uint offset, uint length);

        public DataReader CreateReader() => CreateReader(0U, Length);

        public abstract void Dispose();

        protected DataReader CreateReader(DataStream stream, uint offset, uint length) {
			uint maxOffset = Length;
			if (offset > maxOffset)
				offset = maxOffset;
			if ((ulong)offset + length > maxOffset)
				length = maxOffset - offset;
			return new DataReader(stream, offset, length);
		}

        public virtual event EventHandler DataReaderInvalidated { add { } remove { } }
    }
}