using ILSpy.IO;

namespace ILSpy.Meta
{
    public sealed class PdbStream : HeapStream
    {
        public byte[] Id { get; private set; }

        public MetadataToken EntryPoint { get; private set; }

        public ulong ReferencedTypeSystemTables { get; private set; }

        public uint[] TypeSystemTableRows { get; private set; }

        public PdbStream(DataReaderFactory mdReaderFactory, uint metadataBaseOffset, StreamHeader streamHeader)
            : base(mdReaderFactory, metadataBaseOffset, streamHeader)
        {
            var reader = CreateReader();
            Id = reader.ReadBytes(20);
            EntryPoint = new MetadataToken(reader.ReadUInt32());
            var tables = reader.ReadUInt64();
            ReferencedTypeSystemTables = tables;
            var rows = new uint[64];
            for (int i = 0; i < rows.Length; i++, tables >>= 1)
            {
                if (((uint)tables & 1) != 0)
                {
                    rows[i] = reader.ReadUInt32();
                }
            }
            TypeSystemTableRows = rows;
        }
    }
}