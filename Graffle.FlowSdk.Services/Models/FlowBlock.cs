using System;
using System.Collections.Generic;
using System.Linq;

namespace Graffle.FlowSdk.Services.Models
{
    public sealed class FlowBlock {
        public FlowBlock(Flow.Entities.Block block) {
            this.Height = block.Height;
            this.Id = block.Id.ToHash();
            this.ParentId = block.ParentId.ToHash();
            this.Timestamp = block.Timestamp.ToDateTimeOffset();
            this.CollectionGuarantees = block.CollectionGuarantees.Select(x => new FlowCollectionGuarantee(x));
            this.Signatures = block.Signatures.Select(x => x.ToHash());
            this.BlockSeals = block.BlockSeals.Select(x => new FlowBlockSeal(x));
        }

        public ulong Height { get; }
        public string ParentId { get; }
        public string Id { get; }
        public DateTimeOffset Timestamp { get; }
        public IEnumerable<FlowCollectionGuarantee> CollectionGuarantees { get; }
        public IEnumerable<string> Signatures { get; }
        public IEnumerable<FlowBlockSeal> BlockSeals { get; }
    }
}