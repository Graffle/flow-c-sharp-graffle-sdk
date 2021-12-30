using Google.Protobuf;
using System.Collections.Generic;
using System.Linq;
using Graffle.FlowSdk;

namespace Graffle.FlowSdk.Services.Models {
    public class FlowBlockSeal
    {
        public FlowBlockSeal(Flow.Entities.BlockSeal blockSeal)
        {
            this.BlockId = blockSeal.BlockId.ToHash();
            this.ExecutionReceiptId = blockSeal.ExecutionReceiptId.ToHash();
            this.ExecutionReceiptSignatures = blockSeal.ExecutionReceiptSignatures.Select(x => x.ToHash());
            this.ResultApprovalSignatures = blockSeal.ResultApprovalSignatures.Select(x => x.ToHash());
        }

        public string BlockId { get; private set; }
        public string ExecutionReceiptId { get; private set; }
        public IEnumerable<string> ExecutionReceiptSignatures { get; private set;}
        public IEnumerable<string> ResultApprovalSignatures { get; private set;}
    }
}