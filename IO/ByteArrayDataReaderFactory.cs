namespace ILSpy.IO
{
    public sealed class ByteArrayDataReaderFactory : DataReaderFactory {

        public override uint Length { get { return length; } }
        
        public override string Filename { get { return filename; } }
       
        public static ByteArrayDataReaderFactory Create(byte[] data, string filename) {
			return new ByteArrayDataReaderFactory(data, filename);
		}

        public static DataReader CreateReader(byte[] data) => Create(data, filename: "").CreateReader();

        DataStream stream;
		string filename;
		uint length;
        
		ByteArrayDataReaderFactory(byte[] data, string filename) {
			this.filename = filename;
			length = (uint)data.Length;
			stream = DataStreamFactory.Create(data);
		}

        public override DataReader CreateReader(uint offset, uint length) => CreateReader(stream, offset, length);


        public override void Dispose() {
			stream = EmptyDataStream.Instance;
			length = 0;
			filename = "";
		}
    }
}