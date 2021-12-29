using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.Collections;
using System.Text.Json;

namespace Graffle.FlowSdk.Services.Models
{
    public sealed class FlowTransactionResponseEvent
    {
        public FlowTransactionResponseEvent(Flow.Entities.Event @event, ByteString blockId, JsonSerializerOptions options)
        {
            this.TransactionId = @event.TransactionId;
            this.TransactionIdHash = @event.TransactionId.ToHash();
            this.Payload = @event.Payload.ToString(Encoding.Default);
            this.EventComposite = System.Text.Json.JsonSerializer.Deserialize<GraffleCompositeType>(this.Payload, options);
            this.TransactionIndex = @event.TransactionIndex;
            this.BlockId = blockId;
            this.BlockIdHash = blockId.ToHash();
        }

        public ByteString TransactionId { get; }
        public string TransactionIdHash { get; }
        public string Payload { get; }
        public GraffleCompositeType EventComposite { get; }
        public uint TransactionIndex { get; }
        public ByteString BlockId { get; }
        public string BlockIdHash { get; }
    }
}