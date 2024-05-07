using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.Collections;
using System.Text.Json;
using Newtonsoft.Json;
using Graffle.FlowSdk.Services.Serialization;

namespace Graffle.FlowSdk.Services.Models
{
    public sealed class FlowEvent
    {
        private static readonly JsonSerializerOptions _jsonOptions;
        static FlowEvent()
        {
            _jsonOptions = new JsonSerializerOptions();
            _jsonOptions.Converters.Add(new GraffleCompositeTypeConverter());
        }

        public FlowEvent(Flow.Entities.Event @event, ulong blockHeight, ByteString blockId, DateTimeOffset blockTimestamp, JsonSerializerOptions options)
        {
            this.TransactionId = @event.TransactionId.ToHash();
            this.Payload = @event.Payload.ToString(Encoding.Default);
            this.EventComposite = System.Text.Json.JsonSerializer.Deserialize<GraffleCompositeType>(this.Payload, options);
            this.TransactionIndex = @event.TransactionIndex;
            this.EventIndex = @event.EventIndex;
            this.BlockHeight = blockHeight;
            this.BlockId = blockId.ToHash();
            this.BlockTimestamp = blockTimestamp;
        }

        public FlowEvent(Flow.Entities.Event @event, ulong blockHeight, ByteString blockId, DateTimeOffset blockTimestamp)
        {
            this.TransactionId = @event.TransactionId.ToHash();
            this.Payload = @event.Payload.ToString(Encoding.Default);
            this.EventComposite = CadenceJsonInterpreter.GraffleCompositeFromEventPayload(this.Payload);
            this.TransactionIndex = @event.TransactionIndex;
            this.EventIndex = @event.EventIndex;
            this.BlockHeight = blockHeight;
            this.BlockId = blockId.ToHash();
            this.BlockTimestamp = blockTimestamp;
        }

        [JsonConstructor]
        public FlowEvent(string transactionId, string payload, GraffleCompositeType eventComposite, uint transactionIndex, uint eventIndex, ulong blockHeight, string blockId, DateTimeOffset blockTimestamp)
        {
            TransactionId = transactionId;
            Payload = payload;
            EventComposite = eventComposite;
            TransactionIndex = transactionIndex;
            EventIndex = eventIndex;
            BlockHeight = blockHeight;
            BlockId = blockId;
            BlockTimestamp = blockTimestamp;
        }

        public static List<FlowEvent> Create(RepeatedField<Flow.Access.EventsResponse.Types.Result> eventsResults, CadenceSerializerVersion serializer = CadenceSerializerVersion.Legacy)
        {
            var eventsList = new List<FlowEvent>();
            foreach (var res in eventsResults)
            {
                eventsList.AddRange(
                    res.Events
                        .Select(ev =>
                        {
                            return serializer switch
                            {
                                CadenceSerializerVersion.Legacy =>
                                    new FlowEvent(ev,
                                                    res.BlockHeight,
                                                    res.BlockId,
                                                    res.BlockTimestamp.ToDateTimeOffset(),
                                                    _jsonOptions),
                                CadenceSerializerVersion.Expando =>
                                    new FlowEvent(ev,
                                                    res.BlockHeight,
                                                    res.BlockId,
                                                    res.BlockTimestamp.ToDateTimeOffset()),
                                _ => throw new ArgumentException("Invalid Cadence Serializer", nameof(serializer))
                            };
                        }
                ).ToList());
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

        [JsonProperty("eventIndex")]
        public uint EventIndex { get; }

        [JsonProperty("blockHeight")]
        public ulong BlockHeight { get; }

        [JsonProperty("blockId")]
        public string BlockId { get; }

        [JsonProperty("blockTimestamp")]
        public DateTimeOffset BlockTimestamp { get; }
    }
}