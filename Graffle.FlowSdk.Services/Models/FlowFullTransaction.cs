using Newtonsoft.Json;

namespace Graffle.FlowSdk.Services.Models
{
    public class FlowFullTransaction
    {
        [JsonConstructor]
        public FlowFullTransaction(FlowTransactionResult flowTransactionResult, FlowTransaction flowTransaction)
        {
            FlowTransactionResult = flowTransactionResult;
            FlowTransaction = flowTransaction;
        }

        [JsonProperty("flowTransactionResult")]
        public FlowTransactionResult FlowTransactionResult { get; }

        [JsonProperty("hasTransactionResult")]
        public bool HasTransactionResult => FlowTransactionResult != null;

        [JsonProperty("flowTransaction")]
        public FlowTransaction FlowTransaction { get; }
    }
}