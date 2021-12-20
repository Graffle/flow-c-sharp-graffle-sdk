using Google.Protobuf;

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
        }

        public FlowAddress Address { get; set; }
        public uint KeyId { get; set; }
        public byte[] Signature { get; set; }
    }
}
