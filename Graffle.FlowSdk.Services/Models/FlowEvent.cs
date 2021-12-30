using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.Collections;
using System.Text.Json;

namespace Graffle.FlowSdk.Services.Models
{
    public sealed class FlowEvent {
        public FlowEvent(Flow.Entities.Event @event, ulong blockHeight, ByteString blockId, DateTimeOffset blockTimestamp, JsonSerializerOptions options) {
            this.TransactionId = @event.TransactionId.ToHash();
            this.Payload = @event.Payload.ToString(Encoding.Default);
            this.EventComposite = System.Text.Json.JsonSerializer.Deserialize<GraffleCompositeType>(this.Payload, options);
            this.TransactionIndex = @event.TransactionIndex;
            this.BlockHeight = blockHeight;
            this.BlockId = blockId.ToHash();
            this.BlockTimestamp = blockTimestamp;
        }

        public static List<FlowEvent> Create(RepeatedField<Flow.Access.EventsResponse.Types.Result> eventsResults) {
            //Set up options for deserializing event data
            var options = new JsonSerializerOptions();
            options.Converters.Add(new FlowCompositeTypeConverter());
            options.Converters.Add(new GraffleCompositeTypeConverter());
            options.Converters.Add(new FlowValueTypeConverter());

            var eventsList = new List<FlowEvent>();
            foreach(var b in eventsResults) {
                eventsList.AddRange(
                    b.Events.ToList()
                        .Select(e => new FlowEvent(
                            e, 
                            b.BlockHeight, 
                            b.BlockId, 
                            b.BlockTimestamp.ToDateTimeOffset(), 
                            options))
                );
            }

            return eventsList;
        }

        public string TransactionId { get; }
        public string Payload { get; }
        public GraffleCompositeType EventComposite { get; }
        public uint TransactionIndex { get; }    
        public ulong BlockHeight { get; }
        public string BlockId { get; }
        public DateTimeOffset BlockTimestamp { get; }
    }
}