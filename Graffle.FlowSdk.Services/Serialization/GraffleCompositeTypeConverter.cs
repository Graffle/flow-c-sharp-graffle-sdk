using Graffle.FlowSdk.Services;
using Graffle.FlowSdk.Types;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

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
                switch (rootType.GetString())
                {
                    case "Struct":
                    case "Resource":
                    case "Event":
                    case "Contract":
                    case "Enum":
                        //Break down like we have before to prep the complex object for a recursive call
                        var complexParsedJson = JsonDocument.Parse(item.Values.Last());
                        var complexRoot = complexParsedJson.RootElement.EnumerateObject().ToDictionary(x => x.Name, x => x.Value);
                        var complexRootValue = complexRoot.FirstOrDefault(z => z.Key == "value").Value.EnumerateObject().ToDictionary(z => z.Name, z => z.Value);
                        var complexFields = complexRootValue.FirstOrDefault(z => z.Key == "fields").Value.EnumerateArray().Select(h => h.EnumerateObject().ToDictionary(n => n.Name, n => n.Value.ToString()));
                        var complexCompositeType = DeserializeFlowCadence(complexRootValue.FirstOrDefault().Value.ToString(), complexRoot.FirstOrDefault().Value.ToString(), complexFields);

                        //Place the complex type in its correct position
                        compositeType.Data[item.Values.First().ToCamelCase()] = complexCompositeType.Data;
                        break;
                    case "Dictionary":
                        var myDictionary = (DictionaryType)FlowValueType.CreateFromCadence(rootType.GetString(), item.Values.Last());
                        var myObject = myDictionary.ConvertToObject();
                        compositeType.Data[item.Values.First().ToCamelCase()] = myObject;
                        break;
                    case "Array":
                        var arrayJson = JsonDocument.Parse(item.Values.Last());
                        var arrayRoot = arrayJson.RootElement.EnumerateObject().ToDictionary(x => x.Name, x => x.Value);
                        var arrayFields = root.FirstOrDefault(z => z.Key == "value").Value.EnumerateArray().Select(h => h.EnumerateObject().ToDictionary(n => n.Name, n => n.Value.ToString()));
                        var result = new List<object>();
                        foreach (var arrayField in arrayFields)
                        {
                            var type = arrayField.Values.First();
                            bool isPrimitive = FlowValueType.IsPrimitiveType(type);
                            if (FlowValueType.IsPrimitiveType(type)
                                || type == "Path"
                                || type == "Capability")
                            {
                                // This is a hack to put back together primitives in an array.
                                var x = arrayField.Values.First();
                                var y = arrayField.Values.Last();
                                string z;
                                if (isPrimitive)
                                {
                                    z = $"{{\"type\":\"{x}\",\"value\":\"{y}\"}}";
                                }
                                else
                                {
                                    //value here is a json object instead of a primitive value
                                    z = $"{{\"type\":\"{x}\",\"value\":{y}}}";
                                }

                                dynamic primitiveValue = FlowValueType.CreateFromCadence(z);
                                var data = primitiveValue.Data;
                                result.Add(data);
                            }
                            else if (type == "Type")
                            {
                                //types can have nested json objects
                                //deserialize it with FlowType
                                var nestedType = FlowType.FromJson(arrayField.Values.Last());
                                result.Add(nestedType.Data.Flatten());
                            }
                            else
                            {
                                // dealing with a recursive complex type
                                var arrayFieldRoot = JsonDocument.Parse(arrayField.Values.Last());
                                var arrayFieldRootElements = arrayFieldRoot.RootElement.EnumerateObject().ToDictionary(x => x.Name, x => x.Value);

                                //composite type ie struct, resource, event, etc
                                var arrayItemId = arrayFieldRootElements.FirstOrDefault(z => z.Key == "id").Value.ToString();
                                var arrayItemType = arrayField.FirstOrDefault().Value.ToString();
                                var singleComplexFields = arrayFieldRootElements.FirstOrDefault(z => z.Key == "fields").Value.EnumerateArray().Select(h => h.EnumerateObject().ToDictionary(n => n.Name, n => n.Value.ToString()));
                                var newItem = DeserializeFlowCadence(arrayItemId, arrayItemType, singleComplexFields);

                                //only add the composite type's data to the array
                                result.Add(newItem.Data);
                            }
                        }
                        compositeType.Data[item.Values.First().ToCamelCase()] = result;
                        break;
                    case "Type": //type can contain nested json objects
                        var parsedType = FlowType.FromJson(item.Values.Last());
                        compositeType.Data[item.Values.First().ToCamelCase()] = parsedType.Data.Flatten();
                        break;
                    default:
                        //We are working with a primitive Cadence type so we can use our SDK to convert it into the value we need.
                        var myValue = ((dynamic)FlowValueType.CreateFromCadence(rootType.GetString(), item.Values.Last())).Data;

                        //If we see the type is optional then we need to open the value type below it to assign either null or the value inside to the property
                        if (rootType.GetString() == "Optional")
                        {
                            if (myValue != null) //optional value is non-null
                            {
                                //get the inner object from the optional type
                                FlowValueType innerObject = FlowValueType.Create(((FlowValueType)myValue).Type, myValue.Data);
                                if (FlowValueType.IsCompositeType(innerObject.Type))
                                {
                                    //nested composite type
                                    //add its fields to the dictionary
                                    var composite = innerObject as CompositeType;
                                    myValue = composite.FieldsAsDictionary();
                                }
                                else if (innerObject.Type == "Type")
                                {
                                    //Type can have nested json objects
                                    //flatten it and add to dictionary
                                    var composite = innerObject as FlowType;
                                    myValue = composite.Data.Flatten();
                                }
                                else //primitive, just add it to the dictionary
                                {
                                    myValue = ((dynamic)innerObject).Data;
                                }
                            }
                        }

                        //Pace the value in our result composite object
                        compositeType.Data[item.Values.First().ToCamelCase()] = myValue;
                        break;
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