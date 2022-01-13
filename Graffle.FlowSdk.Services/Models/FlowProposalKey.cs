using Newtonsoft.Json;

namespace Graffle.FlowSdk.Services.Models
{
    public class FlowProposalKey
    {
        public FlowProposalKey()
        {
        }

        public FlowProposalKey(Flow.Entities.Transaction.Types.ProposalKey flowPropsalKey)
        {
            Address = new FlowAddress(flowPropsalKey.Address);
            KeyId = flowPropsalKey.KeyId;
            SequenceNumber = flowPropsalKey.SequenceNumber;
        }

        [JsonConstructor]
        public FlowProposalKey(FlowAddress address, uint keyId, ulong sequenceNumber)
        {
            Address = address;
            KeyId = keyId;
            SequenceNumber = sequenceNumber;
        }

        [JsonProperty("address")]
        public FlowAddress Address { get; set; }

        [JsonProperty("keyId")]
        public uint KeyId { get; set; }

        [JsonProperty("sequenceNumber")]
        public ulong SequenceNumber { get; set; }
    }
}
