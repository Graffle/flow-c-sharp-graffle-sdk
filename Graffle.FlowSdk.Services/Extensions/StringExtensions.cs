using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Graffle.FlowSdk.Services
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string s)
        {
            if (s.Count() == 1)
                return s.ToLowerInvariant();
            if (char.IsLower(s[0]))
                return s;
            var result = char.ToLowerInvariant(s[0]) + s.Substring(1);
            return result;
        }

        internal static byte[] GetHash(this string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        internal static string GetHashString(this string inputString)
        {
            var sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}