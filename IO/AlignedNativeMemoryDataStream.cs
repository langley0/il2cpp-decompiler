using System.Text;

namespace ILSpy.IO
{
    public sealed class AlignedNativeMemoryDataStream : DataStream
    {
        readonly byte[] data;
        

        public AlignedNativeMemoryDataStream(byte[] data)
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
            return (ushort)(data[offset] | (data[offset+1] << 8));
		}

		public override uint ReadUInt32(uint offset) {
            return (uint)(data[offset] | (data[offset+1] << 8) | (data[offset+2] << 16) | (data[offset+3] << 24));
		}

		public override ulong ReadUInt64(uint offset) {
            return BitConverter.ToUInt64(data, (int)offset);
		}

		public override float ReadSingle(uint offset) {
            // little-endian byte array 에서 float 를 읽는다
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