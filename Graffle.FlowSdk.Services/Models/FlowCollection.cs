using System.Collections.Generic;
using System.Linq;

namespace Graffle.FlowSdk.Services.Models
{
    public sealed class FlowCollection
    {
        public FlowCollection(Flow.Entities.Collection collection)
        {
            this.Id = collection.Id.ToHash();
            this.TransactionIds = collection.TransactionIds.Select(s => s.ToHash());
        }
        public string Id { get; }

        public IEnumerable<string> TransactionIds { get; }
    }
}