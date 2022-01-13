using System.Collections.Generic;
using System.Linq;

namespace Graffle.FlowSdk.Services.Models
{
    public sealed class FlowCollectionGuarantee
    {
        public FlowCollectionGuarantee(Flow.Entities.CollectionGuarantee collectionGuarantee)
        {
            this.CollectionId = collectionGuarantee.CollectionId.ToHash();
            if(collectionGuarantee.Signatures != null && collectionGuarantee.Signatures.Any())
                this.Signatures = collectionGuarantee.Signatures.Select(s => s.ToHash());
        }
        public string CollectionId { get; }

        public IEnumerable<string> Signatures { get; } = Enumerable.Empty<string>();
    }
}