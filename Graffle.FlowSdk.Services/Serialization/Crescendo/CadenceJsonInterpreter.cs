using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Graffle.FlowSdk.Services.Serialization
{
    public static class CadenceJsonInterpreter
    {
        private static readonly ExpandoObjectConverter _expando = new();

        /// <summary>
        /// IDictionary<string,object>, ie json object
        /// </summary>
        /// <returns></returns>
        private static readonly Type OBJECT_TYPE = typeof(IDictionary<string, object>);

        /// <summary>
        /// IList<object>, ie json array
        /// </summary>
        /// <returns></returns>
        private static readonly Type ARRAY_TYPE = typeof(IList<object>);

        /// <summary>
        /// Returns an object containing the properties of the cadence json
        /// </summary>
        /// <param name="json">Cadence json string</param>
        /// <param name="preserveDictionaryKeyCasing">True if string casing key values in dictionaries should be presereved (camel case), false otherwise</param>
        /// <returns></returns>
        public static dynamic ObjectFromCadenceJson(string json, bool preserveDictionaryKeyCasing = false)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(json, nameof(json));

            ExpandoObject parsed;
            try
            {
                parsed = JsonConvert.DeserializeObject<ExpandoObject>(json, _expando);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("invalid json", nameof(json), ex);
            }

            return InterpretCadenceExpandoObject(parsed, preserveDictionaryKeyCasing);
        }

        /// <summary>
        /// Returns a GraffleCompositeType with simplified cadence data
        /// </summary>
        /// <param name="eventPayloadJson">Cadence event json payload</param>
        /// <returns></returns>
        internal static GraffleCompositeType GraffleCompositeFromEventPayload(string eventPayloadJson)
        {
            IDictionary<string, object> parsed;
            try
            {
                parsed = JsonConvert.DeserializeObject<ExpandoObject>(eventPayloadJson, _expando);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to parse event payload json", ex);
            }

            if (parsed["value"] is not IDictionary<string, object> value)
            {
                throw new CadenceJsonCastException("Unexpected type for event value field")
                {
                    ExpectedType = OBJECT_TYPE,
                    ActualType = parsed["value"]?.GetType()
                };
            }

            var res = new GraffleCompositeType(parsed["type"].ToString())
            {
                Id = value["id"].ToString(),
                Data = [],
                SerializerVersion = CadenceSerializerVersion.Crescendo
            };

            if (value["fields"] is not IList<object> fields)
            {
                throw new CadenceJsonCastException("Unexpected type for event fields array")
                {
                    ExpectedType = ARRAY_TYPE,
                    ActualType = value["fields"]?.GetType()
                };
            }

            foreach (var field in fields)
            {
                if (field is not IDictionary<string, object> fieldDictionary)
                {
                    throw new CadenceJsonCastException("Unexpected type for individual event field")
                    {
                        ExpectedType = OBJECT_TYPE,
                        ActualType = field?.GetType()
                    };
                }

                res.Data[fieldDictionary["name"].ToString().ToCamelCase()] = InterpretCadenceExpandoObject(fieldDictionary["value"]);
            }

            return res;
        }


        /// <summary>
        /// https://cadence-lang.org/docs/1.0/json-cadence-spec
        /// </summary>
        /// <param name="cadenceObject"></param>
        /// <returns></returns>
        private static dynamic InterpretCadenceExpandoObject(object cadenceObject, bool preserveDictionaryKeyCasing = false)
        {
            if (cadenceObject is not IDictionary<string, object> cadenceObjectDictionary) //aka ExpandoObject
            {
                throw new CadenceJsonCastException("Unexpected type recevied for InterpretCadenceExpandoObject")
                {
                    ExpectedType = OBJECT_TYPE,
                    ActualType = cadenceObject?.GetType()
                };
            }

            if (!cadenceObjectDictionary.TryGetValue("type", out var type))
                throw new Exception("Cadence Type not found in expando dictionary");

            switch (type.ToString())
            {
                case "Struct":
                case "Resource":
                case "Event":
                case "Contract":
                case "Enum":
                    {
                        if (cadenceObjectDictionary["value"] is not IDictionary<string, object> value)
                        {
                            throw new CadenceJsonCastException($"Unexpected type recevied for Composite value object")
                            {
                                ExpectedType = OBJECT_TYPE,
                                ActualType = cadenceObjectDictionary["value"]?.GetType()
                            };
                        }

                        if (value["fields"] is not IList<object> fields)
                        {
                            throw new CadenceJsonCastException($"Unexpected type recevied for Composite fields array")
                            {
                                ExpectedType = ARRAY_TYPE,
                                ActualType = value["fields"]?.GetType()
                            };
                        }

                        var result = new Dictionary<string, dynamic>();
                        foreach (var field in fields)
                        {
                            if (field is not IDictionary<string, object> fieldDictionary)
                            {
                                throw new CadenceJsonCastException("Unexpected type recevied for composite field")
                                {
                                    ExpectedType = OBJECT_TYPE,
                                    ActualType = field?.GetType()
                                };
                            }

                            result.Add(fieldDictionary["name"].ToString(), InterpretCadenceExpandoObject(fieldDictionary["value"], preserveDictionaryKeyCasing));
                        }

                        return result;
                    }
                case "Dictionary":
                    {
                        if (cadenceObjectDictionary["value"] is not IList<object> value)
                        {
                            throw new CadenceJsonCastException("Unexpected type recevied for Dictionary value field")
                            {
                                ExpectedType = ARRAY_TYPE,
                                ActualType = cadenceObjectDictionary["value"]?.GetType()
                            };
                        }

                        Dictionary<string, dynamic> result = [];
                        foreach (var item in value)
                        {
                            if (item is not IDictionary<string, object> itemDictionary)
                            {
                                throw new CadenceJsonCastException("Unexpected type recevied for dictionary key value object")
                                {
                                    ExpectedType = OBJECT_TYPE,
                                    ActualType = item?.GetType()
                                };
                            }

                            var parsedKey = InterpretCadenceExpandoObject(itemDictionary["key"], preserveDictionaryKeyCasing);
                            string keyStr = parsedKey.ToString();

                            result.Add(preserveDictionaryKeyCasing ? keyStr : keyStr.ToCamelCase(), InterpretCadenceExpandoObject(itemDictionary["value"], preserveDictionaryKeyCasing));
                        }
                        return result;
                    }
                case "Array":
                    {
                        if (cadenceObjectDictionary["value"] is not IList<object> value)
                        {
                            throw new CadenceJsonCastException("Unexpected type recevied for Array value field ")
                            {
                                ExpectedType = ARRAY_TYPE,
                                ActualType = cadenceObjectDictionary["value"]?.GetType()
                            };
                        }

                        return value.Select(x => InterpretCadenceExpandoObject(x, preserveDictionaryKeyCasing)).ToList();
                    }
                case "Optional":
                    {
                        var value = cadenceObjectDictionary["value"];
                        return value is null ? null : InterpretCadenceExpandoObject(value, preserveDictionaryKeyCasing);
                    }
                case "Type":
                    {
                        if (cadenceObjectDictionary["value"] is not IDictionary<string, object> value)
                        {
                            throw new CadenceJsonCastException("Unexpected type recevied for cadence Type \"value\" field")
                            {
                                ExpectedType = OBJECT_TYPE,
                                ActualType = cadenceObjectDictionary["value"]?.GetType()
                            };
                        }

                        return CadenceTypeInterpreter.InterpretCadenceType(value["staticType"]);
                    }
                case "Function":
                    {
                        if (cadenceObjectDictionary["value"] is not IDictionary<string, object> value)
                        {
                            throw new CadenceJsonCastException("Unexpected type recevied for Function \"value\" field")
                            {
                                ExpectedType = OBJECT_TYPE,
                                ActualType = cadenceObjectDictionary["value"]?.GetType()
                            };
                        }

                        return new Dictionary<string, dynamic>
                        {
                            { "functionType", CadenceTypeInterpreter.InterpretCadenceType(value["functionType"]) }
                        };
                    }
                case "Void":
                    {
                        return "Void";
                    }
                case "Path":
                    {
                        return ParsePathType(cadenceObjectDictionary["value"]);
                    }
                case "Capability":
                    {
                        if (cadenceObjectDictionary["value"] is not IDictionary<string, object> value)
                        {
                            throw new CadenceJsonCastException("Unexpected type recevied for Capability \"value\" field")
                            {
                                ExpectedType = OBJECT_TYPE,
                                ActualType = cadenceObjectDictionary["value"]?.GetType()
                            };
                        }

                        var res = new Dictionary<string, dynamic>();
                        if (value.TryGetValue("path", out var path)) //old cadence
                        {
                            res.Add("path", ParsePathType(path));
                        }
                        else if (value.TryGetValue("id", out var id)) //cadence 1.0
                        {
                            res.Add("id", id);
                        }

                        res.Add("address", value["address"]);
                        res.Add("borrowType", CadenceTypeInterpreter.InterpretCadenceType(value["borrowType"]));

                        return res;
                    }
                case "Int8":
                case "Int16":
                case "Int32":
                case "Int64":
                    {
                        if (long.TryParse(cadenceObjectDictionary["value"].ToString(), out var value))
                            return value;

                        return cadenceObjectDictionary["value"];
                    }
                case "Word8":
                case "Word16":
                case "Word32":
                case "Word64":
                case "UInt8":
                case "UInt16":
                case "UInt32":
                case "UInt64":
                    {
                        if (ulong.TryParse(cadenceObjectDictionary["value"].ToString(), out var value))
                            return value;

                        return cadenceObjectDictionary["value"];
                    }
                case "Fix64":
                case "UFix64":
                    {
                        if (decimal.TryParse(cadenceObjectDictionary["value"].ToString(), out var value))
                            return value;

                        return cadenceObjectDictionary["value"];
                    }
                case "Int":
                    {
                        //this is for backwards compability, see IntType in flow-c-sharp-sdk repo
                        //in reality Int has no min or max value
                        if (int.TryParse(cadenceObjectDictionary["value"].ToString(), out var intValue))
                            return intValue;
                        else if (long.TryParse(cadenceObjectDictionary["value"].ToString(), out var longValue)) ///value doent fit into 32bits, try 64
                            return longValue;

                        //value too large for 64bit integer just return the original object (string)
                        return cadenceObjectDictionary["value"];
                    }
                case "UInt":
                    {
                        //see above
                        if (uint.TryParse(cadenceObjectDictionary["value"].ToString(), out var uintValue))
                            return uintValue;
                        else if (ulong.TryParse(cadenceObjectDictionary["value"].ToString(), out var ulongValue))
                            return ulongValue;

                        return cadenceObjectDictionary["value"];
                    }
                case "InclusiveRange":
                    {
                        if (cadenceObjectDictionary["value"] is not IDictionary<string, object> range)
                        {
                            throw new CadenceJsonCastException("Unexpected type recevied for InclusiveRange \"value\" field")
                            {
                                ExpectedType = OBJECT_TYPE,
                                ActualType = cadenceObjectDictionary["value"]?.GetType()
                            };
                        }

                        return new Dictionary<string, dynamic>()
                        {
                            { "start" , GetRangeValue(range["start"]) },
                            { "end" , GetRangeValue(range["end"]) },
                            { "step" , GetRangeValue(range["step"]) },
                        };
                    }
                default: //primitive json, string bool number etc
                    {
                        return cadenceObjectDictionary["value"];
                    }
            }
        }

        private static dynamic GetRangeValue(object rangeValue)
        {
            if (rangeValue is not IDictionary<string, object> rangeDict)
            {
                throw new CadenceJsonCastException("Unexpected type for Range object")
                {
                    ExpectedType = OBJECT_TYPE,
                    ActualType = rangeValue?.GetType()
                };
            }

            return new Dictionary<string, dynamic>()
            {
                { "type",  rangeDict["type"]},
                { "value", rangeDict["value"]}
            };
        }

        private static dynamic ParsePathType(object pathObject)
        {
            if (pathObject is string str)
                return str; //backwards compatibility

            if (pathObject is not IDictionary<string, object> path)
            {
                throw new CadenceJsonCastException("Unexpected type recevied for Path object")
                {
                    ExpectedType = OBJECT_TYPE,
                    ActualType = pathObject?.GetType()
                };
            }

            IDictionary<string, object> target = path;
            if (path.TryGetValue("value", out var pathValue)) //check for value field
            {
                if (pathValue is not IDictionary<string, object> pathValueDict)
                {
                    throw new CadenceJsonCastException("Unexpected type received for Path value")
                    {
                        ExpectedType = OBJECT_TYPE,
                        ActualType = pathValue?.GetType()
                    };
                }

                target = pathValueDict;
            }

            return new Dictionary<string, dynamic>
            {
                { "domain", target["domain"] },
                { "identifier", target["identifier"] }
            };
        }
    }
}
