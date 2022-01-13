using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Graffle.FlowSdk.Services.Models
{
    public sealed class FlowCollectionGuarantee
    {
        [JsonConstructor]
        public FlowCollectionGuarantee(string collectionId, IEnumerable<string> signatures)
        {
            CollectionId = collectionId;
            if (signatures != null && signatures.Any())
                this.Signatures = signatures;
        }

        public FlowCollectionGuarantee(Flow.Entities.CollectionGuarantee collectionGuarantee)
        {
            this.CollectionId = collectionGuarantee.CollectionId.ToHash();
            if (collectionGuarantee.Signatures != null && collectionGuarantee.Signatures.Any())
                this.Signatures = collectionGuarantee.Signatures.Select(s => s.ToHash());
        }

        [JsonProperty("collectionId")]
        public string CollectionId { get; }

        [JsonProperty("signatures")]
        public IEnumerable<string> Signatures { get; } = Enumerable.Empty<string>();
    }
}