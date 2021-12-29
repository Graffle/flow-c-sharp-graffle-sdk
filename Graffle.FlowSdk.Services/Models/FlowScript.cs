using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace Graffle.FlowSdk.Services.Models
{
    public class FlowScript
    {
        public FlowScript(string rawScript)
        {
            RawScript = rawScript;
            ScriptHash = rawScript.GetHashString();
        }

        public string RawScript { get; }

        //TODO: This needs to remove variables and standardize them
        public string ScriptHash { get; }


    }

    //TODO: Fix this and pull it out somewhere proper.
    public static class ScriptHasher
    {

        internal static byte[] GetHash(this string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        internal static string GetHashString(this string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}