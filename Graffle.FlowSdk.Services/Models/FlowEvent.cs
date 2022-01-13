using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.Collections;
using System.Text.Json;
using Newtonsoft.Json;

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

        [JsonConstructor]
        public FlowEvent(string transactionId, string payload, GraffleCompositeType eventComposite, uint transactionIndex, ulong blockHeight, string blockId, DateTimeOffset blockTimestamp)
        {
            TransactionId = transactionId;
            Payload = payload;
            EventComposite = eventComposite;
            TransactionIndex = transactionIndex;
            BlockHeight = blockHeight;
            BlockId = blockId;
            BlockTimestamp = blockTimestamp;
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

        [JsonProperty("TransactionId")]
        public string TransactionId { get; }

        [JsonProperty("payload")]
        public string Payload { get; }

        [JsonProperty("eventComposite")]
        public GraffleCompositeType EventComposite { get; }

        [JsonProperty("transactionIndex")]
        public uint TransactionIndex { get; }

        [JsonProperty("blockHeight")]
        public ulong BlockHeight { get; }

        [JsonProperty("blockId")]
        public string BlockId { get; }

        [JsonProperty("blockTimestamp")]
        public DateTimeOffset BlockTimestamp { get; }
    }
}