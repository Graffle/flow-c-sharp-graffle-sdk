using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Graffle.FlowSdk.Types;
using Graffle.FlowSdk;
using Graffle.FlowSdk.Services;
using Graffle.FlowSdk.Services.Extensions;

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
            var rootType = root.FirstOrDefault(z => z.Key == "type").Value;

            //TODO: Use a factory, to support other composite types
            if (rootType.GetString() == "Struct" || rootType.GetString() == "Event")
            {
                var rootValue = root.FirstOrDefault(z => z.Key == "value").Value.EnumerateObject().ToDictionary(z => z.Name, z => z.Value);
                var fields = rootValue.FirstOrDefault(z => z.Key == "fields").Value.EnumerateArray().Select(h => h.EnumerateObject().ToDictionary(n => n.Name, n => n.Value.ToString()));

                var compositeType = new GraffleCompositeType(root.FirstOrDefault().Value.ToString())
                {
                    Id = rootValue.FirstOrDefault().Value.ToString(),
                    Data = new Dictionary<string, dynamic>()
                };
                foreach (var item in fields)
                {
                    var parsedJson = JsonDocument.Parse(item["value"]);
                    var fieldRoot = parsedJson.RootElement.EnumerateObject().ToDictionary(x => x.Name, x => x.Value);
                    var fieldRootType = fieldRoot.FirstOrDefault(z => z.Key == "type").Value;
                    if (fieldRootType.GetString() == "Dictionary")
                    {
                        var myDictionary = (DictionaryType)FlowValueType.CreateFromCadence(fieldRootType.GetString(), item["value"]);
                        var myObject = myDictionary.ConvertToObject();
                        compositeType.Data[item["name"].ToCamelCase()] = myObject;
                    }
                    else if (fieldRootType.GetString() == "Array")
                    {
                        var myArray = (ArrayType)FlowValueType.CreateFromCadence(fieldRootType.GetString(), item["value"]);
                        compositeType.Data[item["name"].ToCamelCase()] = myArray.ToValueData();
                    }
                    else
                    {
                        compositeType.Data[item["name"].ToCamelCase()] = ((dynamic)FlowValueType.CreateFromCadence(fieldRootType.GetString(), item["value"])).Data;
                    }
                }

                return compositeType;
            }
            else if (rootType.GetString() == "Array")
            {
                var rootElement = rss.RootElement.ToString();
                var result = (ArrayType)FlowValueType.CreateFromCadence("Array", rootElement.ToString());
                return result;
            }
            else if (rootType.GetString() == "Dictionary")
            {
                var rootElement = rss.RootElement.ToString();
                var result = (DictionaryType)FlowValueType.CreateFromCadence("Dictionary", rootElement.ToString());
                return result;
            }
            else
            {
                var rootValue = root.FirstOrDefault(z => z.Key == "value").Value.ToString();
                return FlowValueType.Create(rootType.GetString(), rootValue);
            }
        }

        public override void Write(Utf8JsonWriter writer, FlowValueType value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}