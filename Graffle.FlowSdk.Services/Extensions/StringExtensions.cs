using Google.Protobuf;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Graffle.FlowSdk.Services
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string s)
        {
            if (s.Length <= 1)
                return s.ToLowerInvariant();
            if (char.IsLower(s[0]))
                return s;

            return string.Concat(char.ToLowerInvariant(s[0]), s[1..]);
        }

        internal static byte[] GetHash(this string inputString)
        {
            return SHA256.HashData(Encoding.UTF8.GetBytes(inputString));
        }

        internal static string GetHashString(this string inputString)
        {
            var sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public static ByteString HashToByteString(this string str)
        {
            var upper = str.ToUpperInvariant();
            var splitStr = Enumerable
                .Range(0, upper.Length / 2)
                .Select(i => upper.Substring(i * 2, 2)).ToList();
            var bytes = splitStr.Select(b => Convert.ToByte(b, 16)).ToArray();
            return ByteString.CopyFrom(bytes);
        }
    }
}