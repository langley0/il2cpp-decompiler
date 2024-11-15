namespace ILSpy
{
    static class Utils
    {
        internal static bool Equals(byte[]? a, byte[]? b)
        {
            if (a == b)
            {
                return true;
            }
            if (a is null || b is null)
            {
                return false;
            }
            if (a.Length != b.Length)
            {
                return false;
            }
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }
            return true;
        }


        internal static int GetHashCode(byte[]? a)
        {
            if (a is null || a.Length == 0)
            {
                return 0;
            }
            int count = Math.Min(a.Length / 2, 20);
            if (count == 0)
            {
                count = 1;
            }

            uint hash = 0;
            for (int i = 0, j = a.Length - 1; i < count; i++, j--)
            {
                hash ^= a[i] | ((uint)a[j] << 8);
                hash = (hash << 13) | (hash >> 19);
            }
            return (int)hash;
        }

        internal static Version CreateVersionWithNoUndefinedValues(Version a)
        {
            if (a is null)
            {
                return new Version(0, 0, 0, 0);
            }
            return new Version(a.Major, a.Minor, GetDefaultVersionValue(a.Build), GetDefaultVersionValue(a.Revision));
        }

        static int GetDefaultVersionValue(int val) => val == -1 ? 0 : val;

        static int TryParseHexChar(char c)
        {
            if ('0' <= c && c <= '9')
            {
                return c - '0';
            }
            if ('a' <= c && c <= 'f')
            {
                return 10 + c - 'a';
            }
            if ('A' <= c && c <= 'F')
            {
                return 10 + c - 'A';
            }
            return -1;
        }

        internal static byte[]? ParseBytes(string hexString)
        {
            try
            {
                if (hexString.Length % 2 != 0)
                {
                    return null;
                }
                var bytes = new byte[hexString.Length / 2];
                for (int i = 0; i < hexString.Length; i += 2)
                {
                    int upper = TryParseHexChar(hexString[i]);
                    int lower = TryParseHexChar(hexString[i + 1]);
                    if (upper < 0 || lower < 0)
                        return null;
                    bytes[i / 2] = (byte)((upper << 4) | lower);
                }
                return bytes;
            }
            catch
            {
                return null;
            }
        }
    }
}