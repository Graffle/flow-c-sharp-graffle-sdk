using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Graffle.FlowSdk.Services.Models
{
    public sealed class FlowCollection
    {
        public FlowCollection(Flow.Entities.Collection collection)
        {
            this.Id = collection.Id.ToHash();
            this.TransactionIds = collection.TransactionIds.Select(s => s.ToHash());
        }

        [JsonConstructor]
        public FlowCollection(string id, IEnumerable<string> transactionIds)
        {
            Id = id;
            TransactionIds = transactionIds;
        }

        [JsonProperty("id")]
        public string Id { get; }

        [JsonProperty("transactionIds")]
        public IEnumerable<string> TransactionIds { get; }
    }
}