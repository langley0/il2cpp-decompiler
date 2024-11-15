using System.Runtime.Serialization;

namespace ILSpy.IO
{
    [Serializable]
    public sealed class DataReaderException : IOException
    {
        internal DataReaderException(string message) : base(message) { }
    }

}