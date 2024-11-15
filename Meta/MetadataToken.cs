using ILSpy.Interfaces;

namespace ILSpy.Meta
{
    public readonly struct MetadataToken : IMetadataToken
    {
        public const uint RID_MASK = 0x00FFFFFF;
        public const uint RID_MAX = RID_MASK;
        public const int TABLE_SHIFT = 24;

        readonly uint token;

        public static bool operator ==(MetadataToken left, MetadataToken right) => left.CompareTo(right) == 0;
        public static bool operator !=(MetadataToken left, MetadataToken right) => left.CompareTo(right) != 0;
        public static bool operator <(MetadataToken left, MetadataToken right) => left.CompareTo(right) < 0;
        public static bool operator >(MetadataToken left, MetadataToken right) => left.CompareTo(right) > 0;
        public static bool operator <=(MetadataToken left, MetadataToken right) => left.CompareTo(right) <= 0;
        public static bool operator >=(MetadataToken left, MetadataToken right) => left.CompareTo(right) >= 0;

        public static uint ToRID(uint token) => token & RID_MASK;
        public static uint ToRID(int token) => ToRID((uint)token);
        public static TableType ToTableType(uint token) => (TableType)(token >> TABLE_SHIFT);
        public static TableType ToTableType(int token) => ToTableType((uint)token);



        public MetadataToken(uint token) => this.token = token;

        public MetadataToken(int token)
            : this((uint)token)
        {
        }

        public MetadataToken(TableType table, uint rid)
            : this(((uint)table << TABLE_SHIFT) | rid)
        {
        }

        public MetadataToken(TableType table, int rid)
            : this(((uint)table << TABLE_SHIFT) | (uint)rid)
        {
        }

        public TableType TableType => ToTableType(token);

        public uint Rid => ToRID(token);

        public uint Raw => token;

        public uint Token => token;

        public int ToInt32() => (int)token;

        public uint ToUInt32() => token;

        public int CompareTo(IMetadataToken? other) => token.CompareTo(other?.Token);

        public bool Equals(IMetadataToken? other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not MetadataToken)
            {
                return false;
            }
            return Equals((MetadataToken)obj);
        }

        public override int GetHashCode() => (int)token;

    }
}