using Newtonsoft.Json;

namespace Graffle.FlowSdk.Services.Models
{
    public class FlowContract
    {
        [JsonConstructor]
        public FlowContract(string name, string source)
        {
            Name = name;
            Source = source;
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }
    }
}