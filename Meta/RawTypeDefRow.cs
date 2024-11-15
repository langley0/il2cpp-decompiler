namespace ILSpy.Meta
{
    public readonly struct RawTypeDefRow
    {
        public readonly uint Flags;
        public readonly uint Name;
        public readonly uint Namespace;
        public readonly uint Extends;
        public readonly uint FieldList;
        public readonly uint MethodList;

        public RawTypeDefRow(uint Flags, uint Name, uint Namespace, uint Extends, uint FieldList, uint MethodList)
        {
            this.Flags = Flags;
            this.Name = Name;
            this.Namespace = Namespace;
            this.Extends = Extends;
            this.FieldList = FieldList;
            this.MethodList = MethodList;
        }

        public uint this[int index] =>
            index switch
            {
                0 => Flags,
                1 => Name,
                2 => Namespace,
                3 => Extends,
                4 => FieldList,
                5 => MethodList,
                _ => 0,
            };
    }
}