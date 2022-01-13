using Google.Protobuf;
using Newtonsoft.Json;

namespace Graffle.FlowSdk.Services.Models
{
    public class FlowSignature
    {
        public FlowSignature()
        {
        }

        public FlowSignature(Flow.Entities.Transaction.Types.Signature signature)
        {
            Address = new FlowAddress(signature.Address);
            KeyId = signature.KeyId;
            Signature = signature.ToByteArray();
            SignatureHex = signature.Signature_.ToHash();
        }

        [JsonConstructor]
        public FlowSignature(FlowAddress address, uint keyId, byte[] signature, string signatureHex)
        {
            Address = address;
            KeyId = keyId;
            Signature = signature;
            SignatureHex = signatureHex;
        }

        [JsonProperty("address")]
        public FlowAddress Address { get; set; }

        [JsonProperty("keyId")]
        public uint KeyId { get; set; }

        [JsonProperty("signature")]
        public byte[] Signature { get; set; }

        [JsonProperty("signatureHex")]

        public string SignatureHex { get; }
    }
}
