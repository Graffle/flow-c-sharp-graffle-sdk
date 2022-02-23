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
            var json = rss.RootElement.GetRawText();

            return FlowValueType.CreateFromCadence(json) as CompositeType;
        }

        public override void Write(Utf8JsonWriter writer, CompositeType value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}