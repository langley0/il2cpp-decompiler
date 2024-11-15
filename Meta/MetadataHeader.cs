using System.Text;
using ILSpy.IO;
using ILSpy.Enums;

namespace ILSpy.Meta
{
    public sealed class MetadataHeader : FileSection
    {
        readonly uint signature;
        readonly ushort majorVersion;
        readonly ushort minorVersion;
        readonly uint reserved1;
        readonly uint stringLength;
        readonly string versionString;
        readonly FileOffset offset2ndPart;
        readonly StorageFlags flags;
        readonly byte reserved2;
        readonly ushort streams;
        readonly IList<StreamHeader> streamHeaders;
        public uint Signature => signature;
        public ushort MajorVersion => majorVersion;
        public ushort MinorVersion => minorVersion;
        public uint Reserved1 => reserved1;
        public uint StringLength => stringLength;
        public string VersionString => versionString;
        public FileOffset StorageHeaderOffset => offset2ndPart;
        public StorageFlags Flags => flags;
        public byte Reserved2 => reserved2;
        public ushort Streams => streams;
        public IList<StreamHeader> StreamHeaders => streamHeaders;


        public MetadataHeader(DataReader reader)
            : this(reader, CLRRuntimeReaderKind.CLR)
        {
        }

        public MetadataHeader(DataReader reader, CLRRuntimeReaderKind runtime)
        {
            SetStartOffset(reader.CurrentOffset);
            signature = reader.ReadUInt32();
            if (signature != 0x424A5342)
            {
                throw new BadImageFormatException("Invalid metadata header signature");
            }
            majorVersion = reader.ReadUInt16();
            minorVersion = reader.ReadUInt16();
            reserved1 = reader.ReadUInt32();
            stringLength = reader.ReadUInt32();
            versionString = ReadString(ref reader, stringLength, runtime);
            offset2ndPart = (FileOffset)reader.CurrentOffset;
            flags = (StorageFlags)reader.ReadByte();
            reserved2 = reader.ReadByte();
            streams = reader.ReadUInt16();
            streamHeaders = new StreamHeader[streams];
            for (int i = 0; i < streamHeaders.Count; i++)
            {
                // Mono doesn't verify all of these so we can't either
                var sh = new StreamHeader(reader, runtime);
                if ((ulong)sh.Offset + sh.StreamSize > reader.EndOffset)
                {
                    sh = new StreamHeader(0, 0, "<invalid>");
                }
                streamHeaders[i] = sh;
            }
            SetEndOffset(reader.CurrentOffset);
        }

        static string ReadString(ref DataReader reader, uint maxLength, CLRRuntimeReaderKind runtime)
        {
            ulong endOffset = (ulong)reader.CurrentOffset + maxLength;
            if (runtime == CLRRuntimeReaderKind.Mono)
            {
                endOffset = (endOffset + 3) / 4 * 4;
            }
            if (endOffset > reader.EndOffset)
            {
                throw new BadImageFormatException("Invalid MD version string");
            }
            var utf8Bytes = new byte[maxLength];
            uint i;
            for (i = 0; i < maxLength; i++)
            {
                byte b = reader.ReadByte();
                if (b == 0)
                    break;
                utf8Bytes[i] = b;
            }

            // Skip padding bytes
            while (reader.CurrentOffset < (uint)endOffset)
            {
                reader.ReadByte();
            }

            return Encoding.UTF8.GetString(utf8Bytes, 0, (int)i);
        }
    }
}