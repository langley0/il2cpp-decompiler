using System.Text;
using System.Linq;

namespace ILSpy.IO
{
    public class UnalignedNativeMemoryDataStream : DataStream
    {
        byte [] data;
        public UnalignedNativeMemoryDataStream(byte[] data)
        {
            this.data = data; 
        }

        public override void ReadBytes(uint offset, byte[] destination, int length) {
            Array.Copy(data, offset, destination, 0, length);
		}

		public override void ReadBytes(uint offset, byte[] destination, int destinationIndex, int length) {
            Array.Copy(data, offset, destination, destinationIndex, length);
        }

        public override byte ReadByte(uint offset) 
        {
            return data[offset];
        }

        public override ushort ReadUInt16(uint offset) {
            return BitConverter.ToUInt16(data, (int)offset);
		}

		public override uint ReadUInt32(uint offset) {
            return BitConverter.ToUInt32(data, (int)offset);
		}

        public override ulong ReadUInt64(uint offset) {
            return BitConverter.ToUInt64(data, (int)offset);
        }

        public override float ReadSingle(uint offset) {
            return BitConverter.ToSingle(data, (int)offset);
        }

        public override double ReadDouble(uint offset) {
            return BitConverter.ToDouble(data, (int)offset);
                
        }
        
        public override string ReadUtf16String(uint offset, int chars) {
            return Encoding.Unicode.GetString(data, (int)offset, chars * 2);
        }
		public override string ReadString(uint offset, int length, Encoding encoding) {
            return encoding.GetString(data, (int)offset, length);
        }

		public override bool TryGetOffsetOf(uint offset, uint endOffset, byte value, out uint valueOffset) {
			throw new NotImplementedException();
		}
    }
}