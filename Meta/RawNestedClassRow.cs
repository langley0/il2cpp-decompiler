namespace ILSpy.Meta
{
    public readonly struct RawNestedClassRow
    {
        public readonly uint NestedClass;
        public readonly uint EnclosingClass;

        public RawNestedClassRow(uint NestedClass, uint EnclosingClass)
        {
            this.NestedClass = NestedClass;
            this.EnclosingClass = EnclosingClass;
        }

        public uint this[int index] =>
            index switch
            {
                0 => NestedClass,
                1 => EnclosingClass,
                _ => 0,
            };
    }
}