using Google.Protobuf;
using Graffle.FlowSdk;

namespace Graffle.FlowSdk.Services.Models {
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

        public ByteString Value { get; private set; }
        public string HexValue { get; private set; }
    }
}