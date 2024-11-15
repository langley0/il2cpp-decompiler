namespace ILSpy.IO
{
    static class DataReaderFactoryFactory {
        static DataReaderFactoryFactory() {
        }
        
        public static DataReaderFactory Create(string fileName) {
			return ByteArrayDataReaderFactory.Create(File.ReadAllBytes(fileName), fileName);
		}
    }
}