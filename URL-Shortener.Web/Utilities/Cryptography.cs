using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace URL_Shortener.Web.Utilities
{
    public class Cryptography
    {
        private const string AlphabetBase62 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private static readonly Regex SlugRx = new(@"^[0-9A-Za-z]{1,11}$", RegexOptions.Compiled);

        public static string HashSha256(string input)
        {
            var data = SHA256.HashData(Encoding.Unicode.GetBytes(input));

            var hashBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                hashBuilder.Append(data[i].ToString("X2"));
            }

            return hashBuilder.ToString();
        }

        public static long Base62ConvertToDecimal(string base62)
        {
            if (string.IsNullOrEmpty(base62))
                throw new ArgumentNullException(nameof(base62), "Input Base62 string cannot be null or empty.");

            long result = 0;

            foreach (char c in base62)
            {
                int digit = CharToValue(c);
                // Check for overflow before multiplying:
                if (result > (long.MaxValue - digit) / 62)
                    throw new OverflowException($"Value '{base62}' is too large to fit in a 64-bit signed integer.");

                result = result * 62 + digit;
            }

            return result;
        }

        public static bool IsValidBase62Slug(string slug) => SlugRx.IsMatch(slug);

        public static string Base62Encode(long value)
        {
            if (value == 0) return "0";
            Span<char> buf = stackalloc char[11];
            int pos = buf.Length;
            ulong n = (ulong)value;
            while (n > 0)
            {
                buf[--pos] = AlphabetBase62[(int)(n % 62)];
                n /= 62;
            }
            return new string(buf[pos..]);
        }

        private static int CharToValue(char c)
        {
            if (c >= '0' && c <= '9')
                return c - '0';

            if (c >= 'A' && c <= 'Z')
                return c - 'A' + 10;

            if (c >= 'a' && c <= 'z')
                return c - 'a' + 36;

            throw new FormatException($"Invalid Base62 character: '{c}'.");
        }


    }
}