using Graffle.FlowSdk.Types;
using System.Linq;
using System.Text.Json.Serialization;

namespace System.Text.Json
{
    public class FlowValueTypeConverter : JsonConverter<Graffle.FlowSdk.Types.FlowValueType>
    {
        public override FlowValueType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            //Determine what kind of object we are working with and we convert it.
            JsonDocument.TryParseValue(ref reader, out var rss);
            var root = rss.RootElement.EnumerateObject().ToDictionary(x => x.Name, x => x.Value);

            //At this level we can look at the Type field and figure out what we are looking it.
            var rootType = root.FirstOrDefault(z => z.Key == "type").Value.GetString();

            if (FlowValueType.IsCompositeType(rootType))
            {
                return CompositeType.FromJson(rss.RootElement.GetRawText());
            }
            else if (rootType == "Array")
            {
                return ArrayType.FromJson(rss.RootElement.GetRawText());
            }
            else if (rootType == "Dictionary")
            {
                return DictionaryType.FromJson(rss.RootElement.GetRawText());
            }
            else if (rootType == "Type")
            {
                return FlowType.FromJson(rss.RootElement.GetRawText());
            }
            else //primitive
            {
                return FlowValueType.CreateFromCadence(rootType, rss.RootElement.GetRawText());
            }
        }

        public override void Write(Utf8JsonWriter writer, FlowValueType value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}