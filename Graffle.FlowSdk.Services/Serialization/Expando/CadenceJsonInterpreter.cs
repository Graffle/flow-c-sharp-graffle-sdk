using System;
using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Graffle.FlowSdk.Services.Serialization
{
    public static class CadenceJsonInterpreter
    {
        private static readonly JsonConverter _expando = new ExpandoObjectConverter();

        public static object ObjectFromCadenceJson(string json, bool preserveDictionaryKeyCasing = false)
        {
            var parsed = JsonConvert.DeserializeObject<ExpandoObject>(json, _expando);

            return InterpretCadenceExpandoObject(parsed, preserveDictionaryKeyCasing);
        }

        /// <summary>
        /// Returns a GraffleCompositeType with simplified cadence data
        /// </summary>
        /// <param name="eventPayloadJson">Cadence event json payload</param>
        /// <returns></returns>
        public static GraffleCompositeType GraffleCompositeFromEventPayload(string eventPayloadJson)
        {
            var parsed = JsonConvert.DeserializeObject<ExpandoObject>(eventPayloadJson, _expando);

            var dict = (IDictionary<string, object>)parsed;

            if (dict["value"] is not IDictionary<string, object> value)
                throw new Exception($"Unexpected type for event \"value\" field, expected IDictionary<string,object> received {dict["value"]?.GetType()}");

            var type = dict["type"].ToString();
            var id = value["id"].ToString();
            var res = new GraffleCompositeType(type)
            {
                Id = id,
                Data = []
            };

            if (value["fields"] is not IList<object> fields)
                throw new Exception($"Unexpected type for event \"fields\" field, expected IList<object> received {value["fields"]?.GetType()}");

            foreach (var field in fields)
            {
                if (field is not IDictionary<string, object> fieldDictionary)
                    throw new Exception($"Unexpected type for event field, expected IDictionary<string,object> received {field?.GetType()}");

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
        private static dynamic InterpretCadenceExpandoObject(object cadenceObject, bool preserveDictionaryKeyCasing = false)
        {
            if (cadenceObject is not IDictionary<string, object> cadenceObjectDictionary) //aka ExpandoObject
                throw new Exception($"Unexpected type recevied for InterpretCadenceExpandoObject expected IDictionary<string,object> received {cadenceObject?.GetType()}");

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
                            throw new Exception($"Unexpected type recevied for Composite \"value\" field expected IDictionary<string,object> received {cadenceObjectDictionary["value"]?.GetType()}");

                        if (value["fields"] is not IList<object> fields)
                            throw new Exception($"Unexpected type recevied for Composite \"fields\" field expected IList<object> received {value["fields"]?.GetType()}");

                        var result = new Dictionary<string, dynamic>();
                        foreach (var f in fields)
                        {
                            if (f is not IDictionary<string, object> fieldDictionary)
                                throw new Exception($"Unexpected type recevied for composite field expected IDictionary<string,object> received {f?.GetType()}");

                            var name = fieldDictionary["name"].ToString();
                            var innerValue = InterpretCadenceExpandoObject(fieldDictionary["value"]);

                            result.Add(name, innerValue);
                        }

                        return result;
                    }
                case "Dictionary":
                    {
                        if (cadenceObjectDictionary["value"] is not IList<object> value)
                            throw new Exception($"Unexpected type recevied for Dictionary \"value\" field expected IList<object> received {cadenceObjectDictionary["value"]?.GetType()}");

                        Dictionary<string, dynamic> result = [];
                        foreach (var item in value)
                        {
                            if (item is not IDictionary<string, object> itemDictionary)
                                throw new Exception($"Unexpected type recevied for dictionary entry expected IDictionary<string,object> received {item?.GetType()}");

                            var parsedKey = InterpretCadenceExpandoObject(itemDictionary["key"]);
                            var parsedValue = InterpretCadenceExpandoObject(itemDictionary["value"]);

                            string keyStr = parsedKey.ToString();

                            //todo camel case for backwards compat?
                            result.Add(preserveDictionaryKeyCasing ? keyStr : keyStr.ToCamelCase(), parsedValue);
                        }
                        return result;
                    }
                case "Array":
                    {
                        if (cadenceObjectDictionary["value"] is not IList<object> value)
                            throw new Exception($"Unexpected type recevied for Array \"value\" field expected IList<object> received {cadenceObjectDictionary["value"]?.GetType()}");

                        List<dynamic> values = [];
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
                            throw new Exception($"Unexpected type recevied for Type \"value\" field expected IDictionary<string,object> received {cadenceObjectDictionary["value"]?.GetType()}");

                        return CadenceTypeInterpreter.InterpretCadenceType(value["staticType"]);
                    }
                case "Function":
                    {
                        if (cadenceObjectDictionary["value"] is not IDictionary<string, object> value)
                            throw new Exception($"Unexpected type recevied for Function \"value\" field expected IDictionary<string,object> received {cadenceObjectDictionary["value"]?.GetType()}");

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
                            throw new Exception($"Unexpected type recevied for Capability \"value\" field expected IDictionary<string,object> received {cadenceObjectDictionary["value"]?.GetType()}");

                        return new Dictionary<string, dynamic>
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
                default: //primitive json, string bool number etc
                    {
                        return cadenceObjectDictionary["value"];
                    }
            }
        }

        private static object ParsePathType(object pathObject)
        {
            if (pathObject is string str)
                return str; //backwards compatibility

            if (pathObject is not IDictionary<string, object> path)
                throw new Exception($"Unexpected type recevied for path expected IDictionary<string,object> received {pathObject?.GetType()}");

            IDictionary<string, object> target = path;
            if (path.TryGetValue("value", out var pathValue)) //check for value field
            {
                if (pathValue is not IDictionary<string, object> pathValueDict)
                    throw new Exception($"Unexpected type received for path value expected IDictionary<string,object> received {pathValue?.GetType()}");

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
