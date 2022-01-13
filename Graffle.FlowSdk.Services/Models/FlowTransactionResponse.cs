using Google.Protobuf;
using Newtonsoft.Json;

namespace Graffle.FlowSdk.Services.Models
{
    public class FlowTransactionResponse
    {
        public FlowTransactionResponse()
        {
        }

        [JsonConstructor]
        public FlowTransactionResponse(ByteString id)
        {
            Id = id;
        }

        [JsonProperty("id")]
        public ByteString Id { get; set; }
    }
}