using Google.Protobuf;

namespace Graffle.FlowSdk.Services.Models
{
    public class FlowSignature
    {
        public ByteString Address { get; set; }
        public uint KeyId { get; set; }
        public byte[] Signature { get; set; }
    }
}
