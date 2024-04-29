using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Graffle.FlowSdk.Services.Serialization
{
    public class CadenceJsonInterpreter
    {
        private static readonly JsonConverter _expando = new ExpandoObjectConverter();

        public static object ObjectFromVerboseJson(string json)
        {
            var parsed = JsonConvert.DeserializeObject<ExpandoObject>(json, _expando);

            return InterpretCadenceExpandoObject(parsed);
        }

        public static object ObjectFromFlowVerboseType(string json)
        {
            //tood move this to cadence type parser also rename taht class xddd
            var parsed = JsonConvert.DeserializeObject<ExpandoObject>(json, _expando);

            return CadenceTypeInterpreter.InterpretCadenceType(parsed);
        }

        public static GraffleCompositeType GraffleCompositeFromEventPayload(string eventPayloadJson)
        {
            var parsed = JsonConvert.DeserializeObject<ExpandoObject>(eventPayloadJson, _expando);

            var dict = (IDictionary<string, object>)parsed;

            if (dict["value"] is not IDictionary<string, object> value)
                throw new Exception("todo");

            var type = dict["type"].ToString();
            var id = value["id"].ToString();
            var res = new GraffleCompositeType(type)
            {
                Id = id,
                Data = []
            };

            if (value["fields"] is not IList<object> fields)
            {
                throw new Exception("todo");
            }

            foreach (var field in fields)
            {
                if (field is not IDictionary<string, object> fieldDictionary)
                    throw new Exception("todo");

                var name = fieldDictionary["name"].ToString().ToCamelCase();
                var fieldValue = InterpretCadenceExpandoObject(fieldDictionary["value"]);

                res.Data[name] = fieldValue;
            }

            return res;
        }


        /// <summary>
        /// https://cadence-lang.org/docs/1.0/json-cadence-spec
        /// </summary>
        /// <param name="cadenceObject"></param>
        /// <returns></returns>
        private static dynamic InterpretCadenceExpandoObject(object cadenceObject)
        {
            if (cadenceObject is not IDictionary<string, object> cadenceObjectDictionary)
                throw new Exception("todo");

            string type = cadenceObjectDictionary["type"].ToString();

            switch (type)
            {
                case "Struct":
                case "Resource":
                case "Event":
                case "Contract":
                case "Enum":
                    {
                        if (cadenceObjectDictionary["value"] is not IDictionary<string, object> value)
                            throw new Exception("todo");

                        if (value["fields"] is not IList<object> fields)
                            throw new Exception("todo");

                        var result = new Dictionary<string, object>();
                        foreach (var f in fields)
                        {
                            if (f is not IDictionary<string, object> fieldDictionary)
                                throw new Exception("todo");

                            var name = fieldDictionary["name"].ToString();
                            var innerValue = InterpretCadenceExpandoObject(fieldDictionary["value"]);

                            result.Add(name.ToCamelCase(), innerValue);
                        }

                        return result;
                    }
                case "Dictionary":
                    {
                        if (cadenceObjectDictionary["value"] is not IList<object> value)
                            throw new Exception("todo");

                        Dictionary<object, object> result = [];
                        foreach (var item in value)
                        {
                            if (item is not IDictionary<string, object> itemDictionary)
                                throw new Exception("todo");

                            var parsedKey = InterpretCadenceExpandoObject(itemDictionary["key"]);
                            var parsedValue = InterpretCadenceExpandoObject(itemDictionary["value"]);

                            result.Add(parsedKey, parsedValue);
                        }
                        return result;
                    }
                case "Array":
                    {
                        if (cadenceObjectDictionary["value"] is not IList<object> value)
                            throw new Exception("todo");

                        List<object> values = [];
                        foreach (var arrItem in value)
                        {
                            values.Add(InterpretCadenceExpandoObject(arrItem));
                        }

                        return values;
                    }
                case "Optional":
                    {
                        var value = cadenceObjectDictionary["value"];

                        if (value is null)
                            return null;

                        return InterpretCadenceExpandoObject(value);
                    }
                case "Type":
                    {
                        if (cadenceObjectDictionary["value"] is not IDictionary<string, object> value)
                        {
                            throw new Exception("todo");
                        }

                        return CadenceTypeInterpreter.InterpretCadenceType(value["staticType"]);
                    }
                case "Function":
                    {
                        Dictionary<string, object> result = new() { { "type", "Function" } };

                        if (cadenceObjectDictionary["value"] is not IDictionary<string, object> value)
                        {
                            throw new Exception("todo");
                        }

                        result.Add("functionType", CadenceTypeInterpreter.InterpretCadenceType(value["functionType"]));
                        return result;
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
                            throw new Exception("todo");
                        }

                        return new Dictionary<string, object>
                        {
                            { "path", ParsePathType(value["path"]) },
                            { "address", value["address"] }, //str
                            { "borrowType", CadenceTypeInterpreter.InterpretCadenceType(value["borrowType"]) }
                        };
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
                        if (int.TryParse(cadenceObjectDictionary["value"].ToString(), out var value))
                            return value;

                        return cadenceObjectDictionary["value"];
                    }
                default: //primitive json, string bool number etc
                    {
                        return cadenceObjectDictionary["value"];
                    }
            }
        }

        private static Dictionary<string, object> ParsePathType(object pathObject)
        {
            if (pathObject is not IDictionary<string, object> path)
            {
                throw new Exception("todo");
            }

            return new Dictionary<string, object>()
            {
                { "domain", path["domain"] },
                { "identifer", path["identifer"] }
            };
        }
    }
}
