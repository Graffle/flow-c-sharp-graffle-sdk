using Google.Protobuf;
using Graffle.FlowSdk;
using Newtonsoft.Json;

namespace Graffle.FlowSdk.Services.Models
{
    public class FlowAddress
    {
        public FlowAddress(string addressHex)
        {
            Value = addressHex.HexToByteString();
            HexValue = addressHex.RemoveHexPrefix();
        }

        public FlowAddress(ByteString address)
        {
            Value = address.ByteStringToHex().HexToByteString();
            HexValue = address.ByteStringToHex();
        }

        [JsonConstructor]
        public FlowAddress(ByteString value, string hexValue) : this(value)
        {
            HexValue = hexValue;
        }

        [JsonProperty("value")]
        public ByteString Value { get; private set; }

        [JsonProperty("hexValue")]
        public string HexValue { get; private set; }
    }
}