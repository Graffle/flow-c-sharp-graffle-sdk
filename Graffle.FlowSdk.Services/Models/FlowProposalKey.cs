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

        public FlowAddress Address { get; set; }
        public uint KeyId { get; set; }
        public ulong SequenceNumber { get; set; }
    }
}
