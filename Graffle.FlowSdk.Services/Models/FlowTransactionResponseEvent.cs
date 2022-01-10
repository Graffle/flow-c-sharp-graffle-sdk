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
            this.TransactionId = @event.TransactionId.ToHash();
            this.Payload = @event.Payload.ToString(Encoding.Default);
            this.EventComposite = System.Text.Json.JsonSerializer.Deserialize<GraffleCompositeType>(this.Payload, options);
            this.TransactionIndex = @event.TransactionIndex;
            this.BlockId = blockId.ToHash();
            this.Type = @event.Type;
            this.EventIndex = @event.EventIndex;

        }

        public string TransactionId { get; }
        public string Payload { get; }
        public GraffleCompositeType EventComposite { get; }
        public uint TransactionIndex { get; }
        public string BlockId { get; }
        public string Type { get; }

        public uint EventIndex { get; }
    }
}