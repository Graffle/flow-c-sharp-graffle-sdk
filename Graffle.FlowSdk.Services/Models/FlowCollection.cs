using Graffle.FlowSdk.Extensions;
using Google.Protobuf;
using Google.Protobuf.Collections;

public sealed class FlowCollection {
    public FlowCollection(Flow.Entities.Collection collection) {
        this.Id = collection.Id;
        this.IdHash = collection.Id.ToHash();
        this.TransactionIds = collection.TransactionIds;
        this.RawCollection = collection;
    }
    public ByteString Id { get; }
    public string IdHash { get; }
    public RepeatedField<ByteString> TransactionIds { get; }
    public Flow.Entities.Collection RawCollection { get; }
}