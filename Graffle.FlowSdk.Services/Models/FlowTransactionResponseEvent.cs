using System.Text;
using Google.Protobuf;
using System.Text.Json;
using Newtonsoft.Json;
using Graffle.FlowSdk.Services.Serialization;

namespace Graffle.FlowSdk.Services.Models
{
    public sealed class FlowTransactionResponseEvent
    {
        public FlowTransactionResponseEvent(Flow.Entities.Event @event, ByteString blockId, JsonSerializerOptions options)
        {
            this.TransactionId = @event.TransactionId.ToHash();
            this.Payload = @event.Payload.ToString(Encoding.Default);
            this.EventComposite = JsonCadenceInterchangeFormatDeserializer.FromEventPayload(this.Payload);
            //this.EventComposite = System.Text.Json.JsonSerializer.Deserialize<GraffleCompositeType>(this.Payload, options);
            this.TransactionIndex = @event.TransactionIndex;
            this.BlockId = blockId.ToHash();
            this.Type = @event.Type;
            this.EventIndex = @event.EventIndex;
        }

        [JsonConstructor]
        public FlowTransactionResponseEvent(string transactionId, string payload, GraffleCompositeType eventComposite, uint transactionIndex, string blockId, string type, uint eventIndex)
        {
            TransactionId = transactionId;
            Payload = payload;
            EventComposite = eventComposite;
            TransactionIndex = transactionIndex;
            BlockId = blockId;
            Type = type;
            EventIndex = eventIndex;
        }

        [JsonProperty("transactionId")]
        public string TransactionId { get; }

        [JsonProperty("payload")]
        public string Payload { get; }

        [JsonProperty("eventComposite")]
        public GraffleCompositeType EventComposite { get; }

        [JsonProperty("transactionIndex")]
        public uint TransactionIndex { get; }

        [JsonProperty("blockId")]
        public string BlockId { get; }

        [JsonProperty("type")]
        public string Type { get; }

        [JsonProperty("eventIndex")]
        public uint EventIndex { get; }
    }
}