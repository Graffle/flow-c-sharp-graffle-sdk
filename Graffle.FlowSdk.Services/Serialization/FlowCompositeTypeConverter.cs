using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Graffle.FlowSdk.Types;

namespace System.Text.Json
{
    public class FlowCompositeTypeConverter : JsonConverter<Graffle.FlowSdk.Types.CompositeType>
    {
        public override CompositeType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonDocument.TryParseValue(ref reader, out var rss);
            var root = rss.RootElement.EnumerateObject().ToDictionary(x => x.Name, x => x.Value);
            var rootValue = root.FirstOrDefault(z => z.Key == "value").Value.EnumerateObject().ToDictionary(z => z.Name, z => z.Value);
            var fields = rootValue.FirstOrDefault(z => z.Key == "fields").Value.EnumerateArray().Select(h => h.EnumerateObject().ToDictionary(n => n.Name, n => n.Value.ToString()));

            var compositeType = new CompositeType(root.FirstOrDefault().Value.ToString())
            {
                Id = rootValue.FirstOrDefault().Value.ToString(),
                Data = new Dictionary<string, string>()
            };
            foreach (var item in fields)
            {
                compositeType.Data.Add(item.Values.First(), item.Values.Last());
            }

            return compositeType;
        }

        public override void Write(Utf8JsonWriter writer, CompositeType value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}