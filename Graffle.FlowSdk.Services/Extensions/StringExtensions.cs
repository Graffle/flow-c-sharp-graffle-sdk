using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}