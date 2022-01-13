using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Graffle.FlowSdk.Services.Models
{
    public class FlowBlockSeal
    {
        public FlowBlockSeal(Flow.Entities.BlockSeal blockSeal)
        {
            this.BlockId = blockSeal.BlockId.ToHash();
            this.ExecutionReceiptId = blockSeal.ExecutionReceiptId.ToHash();
            this.ExecutionReceiptSignatures = blockSeal.ExecutionReceiptSignatures.Select(x => x.ToHash());
            this.ResultApprovalSignatures = blockSeal.ResultApprovalSignatures.Select(x => x.ToHash());
        }

        [JsonConstructor]
        public FlowBlockSeal(string blockId, string executionReceiptId, IEnumerable<string> executionReceiptSignatures, IEnumerable<string> resultApprovalSignatures)
        {
            BlockId = blockId;
            ExecutionReceiptId = executionReceiptId;
            ExecutionReceiptSignatures = executionReceiptSignatures;
            ResultApprovalSignatures = resultApprovalSignatures;
        }

        [JsonProperty("blockId")]
        public string BlockId { get; private set; }

        [JsonProperty("executionReceiptId")]
        public string ExecutionReceiptId { get; private set; }

        [JsonProperty("executionReceiptSignatures")]
        public IEnumerable<string> ExecutionReceiptSignatures { get; private set; }

        [JsonProperty("resultApprovalSignatures")]
        public IEnumerable<string> ResultApprovalSignatures { get; private set; }
    }
}