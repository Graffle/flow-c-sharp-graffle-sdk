using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Graffle.FlowSdk.Types;
using Graffle.FlowSdk;
using Graffle.FlowSdk.Services;

namespace System.Text.Json
{
    public class GraffleCompositeType : FlowValueType
    {
        public GraffleCompositeType()
        {
        }
        public GraffleCompositeType(string type)
        {
            Type = type;
        }

        public override string Type { get; }
        public string Id { get; set; }

        public Dictionary<string, dynamic> Data { get; set; }

        public override string AsJsonCadenceDataFormat()
        {
            throw new NotImplementedException();
        }
        public override string DataAsJson()
        {
            throw new NotImplementedException();
        }
    }

    public static class DictionaryTypeExtension
    {
        public static dynamic ConvertToObject(this DictionaryType x)
        {
            var result = new Dictionary<dynamic, dynamic>();
            foreach (var item in x.Data)
            {
                string propertyName = ((dynamic)item.Key).Data;
                var cleanedName = propertyName.ToCamelCase();
                result[cleanedName] = ((dynamic)item.Value).Data;
            }
            return result;
        }
    }

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
                    var parsedJson = JsonDocument.Parse(item.Values.Last());
                    var fieldRoot = parsedJson.RootElement.EnumerateObject().ToDictionary(x => x.Name, x => x.Value);
                    var fieldRootType = fieldRoot.FirstOrDefault(z => z.Key == "type").Value;
                    if (fieldRootType.GetString() == "Dictionary")
                    {
                        var myDictionary = (DictionaryType)FlowValueType.CreateFromCadence(fieldRootType.GetString(), item.Values.Last());
                        var myObject = myDictionary.ConvertToObject();
                        compositeType.Data[item.Values.First().ToCamelCase()] = myObject;
                    }
                    else if (fieldRootType.GetString() == "Array")
                    {
                        var myArray = (ArrayType)FlowValueType.CreateFromCadence(fieldRootType.GetString(), item.Values.Last());
                        compositeType.Data[item.Values.First().ToCamelCase()] = myArray.ToValueData();
                    }
                    else
                    {
                        compositeType.Data[item.Values.First().ToCamelCase()] = ((dynamic)FlowValueType.CreateFromCadence(fieldRootType.GetString(), item.Values.Last())).Data;
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