using System.Text;
using ILSpy.IO;
using ILSpy.Enums;

namespace ILSpy.Meta
{
    public sealed class StreamHeader : FileSection
    {
        readonly uint offset;
        readonly uint streamSize;
        readonly string name;
        public uint Offset => offset;
        public uint StreamSize => streamSize;
        public string Name => name;

        public StreamHeader(DataReader reader)
            : this(reader, CLRRuntimeReaderKind.CLR)
        {
        }

        internal StreamHeader(DataReader reader, CLRRuntimeReaderKind runtime)
        {

            SetStartOffset(reader.CurrentOffset);
            offset = reader.ReadUInt32();
            streamSize = reader.ReadUInt32();
            name = ReadString(reader, 32);
            SetEndOffset(reader.CurrentOffset);

            if (runtime == CLRRuntimeReaderKind.Mono)
            {
                if (offset > reader.Length)
                {
                    offset = reader.Length;
                }
                // Mono ignores the size (eg. it can be 0 or max value) so set it to the max possible value
                streamSize = reader.Length - offset;
            }

            var size = EndOffset - StartOffset;
            if (offset + size < offset)
            {
                throw new BadImageFormatException("Invalid stream header");
            }
        }

        internal StreamHeader(uint offset, uint streamSize, string name)
        {
            this.offset = offset;
            this.streamSize = streamSize;
            this.name = name ?? throw new ArgumentNullException(nameof(name));
        }

        static string ReadString(DataReader reader, int maxLen)
        {
            var origPos = reader.Position;
            var sb = new StringBuilder(maxLen);
            int i;
            for (i = 0; i < maxLen; i++)
            {
                byte b = reader.ReadByte();
                if (b == 0)
                {
                    break;
                }
                sb.Append((char)b);
            }
            if (i == maxLen)
            {
                // overflow
                throw new BadImageFormatException("Invalid stream header");
            }
            if (i != maxLen)
            {
                reader.Position = origPos + (((uint)i + 1 + 3) & ~3U);
            }
            return sb.ToString();
        }
    }
}