namespace ILSpy.Meta
{
    public readonly struct RawFieldRow
    {
        public readonly ushort Flags;
        public readonly uint Name;
        public readonly uint Signature;

        public RawFieldRow(ushort Flags, uint Name, uint Signature)
        {
            this.Flags = Flags;
            this.Name = Name;
            this.Signature = Signature;
        }

        public uint this[int index] =>
            index switch
            {
                0 => Flags,
                1 => Name,
                2 => Signature,
                _ => 0,
            };
    }
}