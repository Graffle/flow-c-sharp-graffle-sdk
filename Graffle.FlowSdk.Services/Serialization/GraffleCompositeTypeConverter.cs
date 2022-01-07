using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Graffle.FlowSdk.Types;
using Graffle.FlowSdk;
using Graffle.FlowSdk.Services;

namespace System.Text.Json
{
    public class GraffleCompositeTypeConverter : JsonConverter<GraffleCompositeType>
    {
        /// <summary>
        /// This function will recursively break down primitive and complex objects into a Graffle Composite Type Object.
        /// </summary>
        /// <param name="id">This is the id in the event/structure/complex object</param>
        /// <param name="flowType">The Type of object the Cadence object represents</param>
        /// <param name="fields">All the data needed converted from Cadence to dynamic types</param>
        /// <returns></returns>
        public GraffleCompositeType DeserializeFlowCadence(string id, string flowType, IEnumerable<Dictionary<string, string>> fields)
        {
            //Create the base composite type where we cans tore our results
            var compositeType = new GraffleCompositeType(flowType)
            {
                Id = id,
                Data = new Dictionary<string, dynamic>()
            };

            //Loop through all the fields and convert them to the proper dictionary dynamic
            foreach (var item in fields)
            {
                //Parse the json out so we can traverse the elements
                var parsedJson = JsonDocument.Parse(item.Values.Last());

                //convert the json into a root dictionary for us to look through to find the value we need
                var root = parsedJson.RootElement.EnumerateObject().ToDictionary(x => x.Name, x => x.Value);

                //We get the type out first so we know what type of cadence object we are working with.
                var rootType = root.FirstOrDefault(z => z.Key == "type").Value;
                //Check to see if we have a complex type like a Struct. If we do then we need to parse a little further and recursively call this function.
                // If not we have either an option or a primitive type.
                if (rootType.GetString() == "Struct")
                {
                    //Break down like we have before to prep the complex object for a recursive call
                    var complexParsedJson = JsonDocument.Parse(item.Values.Last());
                    var complexRoot = complexParsedJson.RootElement.EnumerateObject().ToDictionary(x => x.Name, x => x.Value);
                    var complexRootValue = complexRoot.FirstOrDefault(z => z.Key == "value").Value.EnumerateObject().ToDictionary(z => z.Name, z => z.Value);
                    var complexFields = complexRootValue.FirstOrDefault(z => z.Key == "fields").Value.EnumerateArray().Select(h => h.EnumerateObject().ToDictionary(n => n.Name, n => n.Value.ToString()));
                    var complexCompositeType = DeserializeFlowCadence(complexRootValue.FirstOrDefault().Value.ToString(), complexRoot.FirstOrDefault().Value.ToString(), complexFields);

                    //Place the complex type in its correct position
                    compositeType.Data[item.Values.First().ToCamelCase()] = complexCompositeType.Data;
                }
                else if (rootType.GetString() == "Dictionary")
                {
                    var myDictionary = (DictionaryType)FlowValueType.CreateFromCadence(rootType.GetString(), item.Values.Last());
                    var myObject = myDictionary.ConvertToObject();
                    compositeType.Data[item.Values.First().ToCamelCase()] = myObject;
                }
                else
                {
                    //We are working with a primitive Cadence type so we can use our SDK to convert it into the value we need.
                    var myValue = ((dynamic)FlowValueType.CreateFromCadence(rootType.GetString(), item.Values.Last())).Data;

                    //If we see the type is optional then we need to open the value type below it to assign either null or the value inside to the property
                    if (rootType.GetString() == "Optional")
                    {
                        if (myValue != null)
                            myValue = ((dynamic)FlowValueType.Create(((FlowValueType)myValue).Type, myValue.Data)).Data;
                    }

                    //Pace the value in our result composite object
                    compositeType.Data[item.Values.First().ToCamelCase()] = myValue;
                }
            }
            return compositeType;
        }

        public override GraffleCompositeType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            //Parse the Cadence Json
            JsonDocument.TryParseValue(ref reader, out var rss);

            //Convert to a Dictionary so we can get access to the fields
            var root = rss.RootElement.EnumerateObject().ToDictionary(x => x.Name, x => x.Value);

            //Get the root value so we know the id
            var rootValue = root.FirstOrDefault(z => z.Key == "value").Value.EnumerateObject().ToDictionary(z => z.Name, z => z.Value);

            //Get all the fields in the dictionary to be processed
            var fields = rootValue.FirstOrDefault(z => z.Key == "fields").Value.EnumerateArray().Select(h => h.EnumerateObject().ToDictionary(n => n.Name, n => n.Value.ToString()));

            // recursively break down the cadence objects into a dynamic dictionary
            var compositeType = DeserializeFlowCadence(rootValue.FirstOrDefault().Value.ToString(), root.FirstOrDefault().Value.ToString(), fields);

            // WE DID IT!
            return compositeType;
        }

        public override void Write(Utf8JsonWriter writer, GraffleCompositeType value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}