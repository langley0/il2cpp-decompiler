using System.Text;

namespace ILSpy.IO
{
    public sealed class EmptyDataStream : DataStream
    {
        public static readonly DataStream Instance = new EmptyDataStream();

        public EmptyDataStream()
        {
        }

        public override void ReadBytes(uint offset, byte[] destination, int length)
        {
            for (int i = 0; i < length; i++)
            {
                destination[i] = 0;
            }
        }
        public override void ReadBytes(uint offset, byte[] destination, int destinationIndex, int length)
        {
            for (int i = 0; i < length; i++)
            {
                destination[destinationIndex + i] = 0;
            }
        }
        public override byte ReadByte(uint offset) => 0;
        public override ushort ReadUInt16(uint offset) => 0;
        public override uint ReadUInt32(uint offset) => 0;
        public override ulong ReadUInt64(uint offset) => 0;
        public override float ReadSingle(uint offset) => 0;
        public override double ReadDouble(uint offset) => 0;
        public override string ReadUtf16String(uint offset, int chars) => string.Empty;
        public override string ReadString(uint offset, int length, Encoding encoding) => string.Empty;
        public override bool TryGetOffsetOf(uint offset, uint endOffset, byte value, out uint valueOffset)
        {
            valueOffset = 0;
            return false;
        }
    }
}