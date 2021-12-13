using System;
using Graffle.FlowSdk.Extensions;
using Google.Protobuf;

public sealed class FlowBlock {
    public FlowBlock(Flow.Entities.Block block) {
        this.Height = block.Height;
        this.Id = block.Id;
        this.IdHash = block.Id.ToHash();
        this.ParentId = block.ParentId;
        this.ParentIdHash = block.ParentId.ToHash();
        this.Timestamp = block.Timestamp.ToDateTimeOffset();
        this.RawBlock = block;
    }

    public ulong Height { get; }
    public ByteString ParentId { get; }
    public string ParentIdHash { get; }
    public ByteString Id { get; }
    public string IdHash { get; }
    public DateTimeOffset Timestamp { get; }
    public Flow.Entities.Block RawBlock { get; }
}