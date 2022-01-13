using Newtonsoft.Json;

namespace Graffle.FlowSdk.Services.Models
{
    public class FlowFullTransaction
    {
        [JsonConstructor]
        public FlowFullTransaction(IFlowTransactionResult flowTransactionResult, IFlowTransaction flowTransaction)
        {
            FlowTransactionResult = flowTransactionResult;
            FlowTransaction = flowTransaction;
        }

        [JsonProperty("flowTransactionResult")]
        public IFlowTransactionResult FlowTransactionResult { get; }

        [JsonProperty("flowTransaction")]
        public IFlowTransaction FlowTransaction { get; }
    }
}