using System;
using Google.Protobuf;

namespace Graffle.FlowSdk.Services
{
    public static class ByteStringExtensions
    {
        public static string ToHash(this ByteString byteString)
        {
            var bytes = byteString.ToByteArray();
            string hashString = BitConverter.ToString(bytes);
            var result = hashString.Replace("-", "").ToLowerInvariant();
            return result;
        }
    }
}