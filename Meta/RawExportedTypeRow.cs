namespace ILSpy.Meta
{
    public readonly struct RawExportedTypeRow
    {
        public readonly uint Flags;
        public readonly uint TypeDefId;
        public readonly uint TypeName;
        public readonly uint TypeNamespace;
        public readonly uint Implementation;

        public RawExportedTypeRow(uint Flags, uint TypeDefId, uint TypeName, uint TypeNamespace, uint Implementation)
        {
            this.Flags = Flags;
            this.TypeDefId = TypeDefId;
            this.TypeName = TypeName;
            this.TypeNamespace = TypeNamespace;
            this.Implementation = Implementation;
        }

        public uint this[int index] =>
            index switch
            {
                0 => Flags,
                1 => TypeDefId,
                2 => TypeName,
                3 => TypeNamespace,
                4 => Implementation,
                _ => 0,
            };
    }
}