using System.Globalization;
using System.Text;

namespace ILSpy
{
    public sealed class UTF8String : IEquatable<UTF8String>, IComparable<UTF8String>
    {
        public static readonly UTF8String Empty = new(string.Empty);


        class UTF8StringEqualityComparer : IEqualityComparer<UTF8String>
        {
            public static readonly UTF8StringEqualityComparer Instance = new();

            /// <inheritdoc/>
            public bool Equals(UTF8String? x, UTF8String? y) => UTF8String.Equals(x, y);

            /// <inheritdoc/>
            public int GetHashCode(UTF8String obj) => UTF8String.GetHashCode(obj);
        }

        public static string ToSystemStringOrEmpty(UTF8String utf8) => ToSystemString(utf8) ?? string.Empty;

        public static bool operator ==(UTF8String left, UTF8String right) => CompareTo(left, right) == 0;

        public static bool operator ==(UTF8String left, string right) => ToSystemString(left) == right;

        public static bool operator ==(string left, UTF8String right) => left == ToSystemString(right);

        public static bool operator !=(UTF8String left, UTF8String right) => CompareTo(left, right) != 0;

        public static bool operator !=(UTF8String left, string right) => ToSystemString(left) != right;

        public static bool operator !=(string left, UTF8String right) => left != ToSystemString(right);

        public static bool operator >(UTF8String left, UTF8String right) => CompareTo(left, right) > 0;

        public static bool operator <(UTF8String left, UTF8String right) => CompareTo(left, right) < 0;

        public static bool operator >=(UTF8String left, UTF8String right) => CompareTo(left, right) >= 0;

        public static bool operator <=(UTF8String left, UTF8String right) => CompareTo(left, right) <= 0;

		public static implicit operator string(UTF8String s) => ToSystemString(s) ?? string.Empty;

		public static implicit operator UTF8String(string s) => new(s);

        public static bool Equals(UTF8String? a, UTF8String? b) => CompareTo(a, b) == 0;

        public static int CompareTo(UTF8String? a, UTF8String? b) => CompareTo(a?.data, b?.data);

        internal static int CompareTo(byte[]? a, byte[]? b)
        {
            if (a == b)
            {
                return 0;
            }
            if (a is null)
            {
                return -1;
            }
            if (b is null)
            {
                return 1;
            }

            int count = Math.Min(a.Length, b.Length);
            for (int i = 0; i < count; i++)
            {
                var ai = a[i];
                var bi = b[i];
                if (ai < bi)
                {
                    return -1;
                }
                if (ai > bi)
                {
                    return 1;
                }
            }
            return a.Length.CompareTo(b.Length);
        }

        public static string? ToSystemString(UTF8String utf8)
        {
            if (utf8 is null || utf8.data is null)
            {
                return null;
            }
            if (utf8.data.Length == 0)
            {
                return string.Empty;
            }
            return utf8.String;
        }

        static string? ConvertFromUTF8(byte[]? data)
        {
            if (data is null)
                return null;
            try
            {
                return Encoding.UTF8.GetString(data);
            }
            catch
            {
            }
            return null;
        }

        public static bool IsNullOrEmpty(UTF8String utf8)
        {
            return utf8 is null || utf8.data is null || utf8.data.Length == 0;
        }

        public static int GetHashCode(UTF8String utf8)
        {
            if (utf8.data is byte[] data)
            {
                return Utils.GetHashCode(data);
            }
            return 0;
        }


        readonly byte[]? data;
        string? asString;

        public UTF8String(byte[]? data) {
            this.data = data;
        } 

        public UTF8String(string s)
			: this(s is null ? null : Encoding.UTF8.GetBytes(s)) {
		}

        public string String
        {
            get
            {
                asString ??= ConvertFromUTF8(data);
                asString ??= string.Empty;
                return asString;
            }
        }


        public bool Equals(UTF8String? other) => CompareTo(this, other) == 0;

        public override bool Equals(object? obj)
        {
            if (obj is UTF8String other)
            {
                return CompareTo(this, other) == 0;
            }
            return false;
        }

        public int CompareTo(UTF8String? other) => CompareTo(this, other);

        public override int GetHashCode() => GetHashCode(this);

        public override string ToString() => String;


        public bool Contains(string value) => String.Contains(value);

		public bool EndsWith(string value) => String.EndsWith(value);

		public bool EndsWith(string value, bool ignoreCase, CultureInfo culture) => String.EndsWith(value, ignoreCase, culture);

		public bool EndsWith(string value, StringComparison comparisonType) => String.EndsWith(value, comparisonType);

		public bool StartsWith(string value) => String.StartsWith(value);

		public bool StartsWith(string value, bool ignoreCase, CultureInfo culture) => String.StartsWith(value, ignoreCase, culture);

		public bool StartsWith(string value, StringComparison comparisonType) => String.StartsWith(value, comparisonType);
    }
}