namespace ILSpy.Meta
{
    public readonly struct RawMethodRow
    {
        public readonly uint RVA;
        public readonly ushort ImplFlags;
        public readonly ushort Flags;
        public readonly uint Name;
        public readonly uint Signature;
        public readonly uint ParamList;

        public RawMethodRow(uint RVA, ushort ImplFlags, ushort Flags, uint Name, uint Signature, uint ParamList)
        {
            this.RVA = RVA;
            this.ImplFlags = ImplFlags;
            this.Flags = Flags;
            this.Name = Name;
            this.Signature = Signature;
            this.ParamList = ParamList;
        }

        public uint this[int index] =>
            index switch
            {
                0 => RVA,
                1 => ImplFlags,
                2 => Flags,
                3 => Name,
                4 => Signature,
                5 => ParamList,
                _ => 0,
            };
    }
}