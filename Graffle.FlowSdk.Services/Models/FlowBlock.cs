using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

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

        [JsonConstructor]
        public FlowBlock(ulong height, string parentId, string id, DateTimeOffset timestamp, IEnumerable<FlowCollectionGuarantee> collectionGuarantees, IEnumerable<string> signatures, IEnumerable<FlowBlockSeal> blockSeals)
        {
            Height = height;
            ParentId = parentId;
            Id = id;
            Timestamp = timestamp;
            CollectionGuarantees = collectionGuarantees;
            Signatures = signatures;
            BlockSeals = blockSeals;
        }

        [JsonProperty("height")]
        public ulong Height { get; }

        [JsonProperty("parentId")]
        public string ParentId { get; }

        [JsonProperty("id")]
        public string Id { get; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; }

        [JsonProperty("collectionGuarantees")]
        public IEnumerable<FlowCollectionGuarantee> CollectionGuarantees { get; }

        [JsonProperty("signatures")]
        public IEnumerable<string> Signatures { get; }

        [JsonProperty("blockSeals")]
        public IEnumerable<FlowBlockSeal> BlockSeals { get; }
    }
}