using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.Collections;

namespace Graffle.FlowSdk.Services.Models
{
    public sealed class FlowCollection {
        public FlowCollection(Flow.Entities.Collection collection) {
            this.Id = collection.Id;
            this.IdHash = collection.Id.ToHash();
            this.TransactionIds = collection.TransactionIds;
            this.RawCollection = collection;
            this.TransactionIdHashes = collection.TransactionIds.Select(s=> s.ToHash());
        }
        public ByteString Id { get; }
        public string IdHash { get; }
        public RepeatedField<ByteString> TransactionIds { get; }

        public IEnumerable<string> TransactionIdHashes{get;}
        public Flow.Entities.Collection RawCollection { get; }
    }
}