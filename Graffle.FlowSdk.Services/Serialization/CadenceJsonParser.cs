using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Graffle.FlowSdk.Services.Serialization
{
    public class JsonCadenceInterchangeFormatParser
    {
        private static readonly JsonConverter _expando = new ExpandoObjectConverter();

        public static object ObjectFromVerboseJson(string json)
        {
            var parsed = JsonConvert.DeserializeObject<ExpandoObject>(json, _expando);

            return ParseVerboseType(parsed);
        }

        public static object ObjectFromFlowVerboseType(string json)
        {
            //tood move this to cadence type parser also rename taht class xddd
            var parsed = JsonConvert.DeserializeObject<ExpandoObject>(json, _expando);

            return CadenceTypeParser.ParseFlowType(parsed);
        }

        public static GraffleCompositeType FromEventPayload(string json)
        {
            var parsed = JsonConvert.DeserializeObject<ExpandoObject>(json, _expando);

            var dict = (IDictionary<string, object>)parsed;

            var type = dict["type"].ToString();

            if (dict["value"] is not ExpandoObject value)
                throw new Exception("todo");

            var valueDict = (IDictionary<string, object>)value;

            var id = valueDict["id"].ToString();

            if (valueDict["fields"] is not IList<object> fields)
            {
                throw new Exception("todo");
            }

            var res = new GraffleCompositeType(type)
            {
                Id = id,
                Data = []
            };

            foreach (var field in fields)
            {
                if (field is not ExpandoObject fieldObj)
                    throw new Exception("todo");

                var parsedField = ParseEventField(fieldObj);
                res.Data[parsedField.name] = parsedField.value;
            }

            return res;
        }

        /// <summary>
        /// https://cadence-lang.org/docs/1.0/json-cadence-spec
        /// </summary>
        /// <param name="flowVerboseType"></param>
        /// <returns></returns>
        private static dynamic ParseVerboseType(object flowVerboseType)
        {
            if (flowVerboseType is not IDictionary<string, object> fieldValue)
                throw new Exception("todo");

            string type = fieldValue["type"].ToString();
            object value = fieldValue["value"];

            switch (type)
            {
                case "Struct":
                case "Resource":
                case "Event":
                case "Contract":
                case "Enum":
                    {
                        var result = new Dictionary<string, object>();

                        if (value is not IDictionary<string, object> compositeObj)
                            throw new Exception("todo");

                        result.Add("id", compositeObj["id"]);

                        if (compositeObj["fields"] is not IList<object> fieldsArr)
                            throw new Exception("todo");

                        foreach (var f in fieldsArr)
                        {
                            if (f is not IDictionary<string, object> fieldObj)
                                throw new Exception("todo");

                            var name = fieldObj["name"].ToString();
                            var innerValue = ParseVerboseType(fieldObj["value"]);

                            result.Add(name.ToCamelCase(), innerValue);
                        }

                        return result;
                    }
                case "Dictionary":
                    {
                        if (value is not IList<object> dict)
                            throw new Exception("todo");

                        Dictionary<object, object> result = [];
                        foreach (var dictValue in dict)
                        {
                            if (dictValue is not IDictionary<string, object> innerObj)
                                throw new Exception("todo");

                            var parsedKey = ParseVerboseType(innerObj["key"]);
                            var parsedValue = ParseVerboseType(innerObj["value"]);

                            result.Add(parsedKey, parsedValue);
                        }
                        return result;
                    }
                case "Array":
                    {
                        if (value is not IList<object> arr)
                            throw new Exception("todo");

                        List<object> values = [];
                        foreach (var arrValue in arr)
                        {
                            values.Add(ParseVerboseType(arrValue));
                        }

                        return values;
                    }
                case "Optional":
                    {
                        if (value is null)
                            return null;

                        return ParseVerboseType(value);
                    }
                case "Type":
                    {
                        Dictionary<string, object> result = [];
                        result.Add("type", "Type");
                        result.Add("staticType", CadenceTypeParser.ParseFlowType(value));
                        return result;
                    }
                case "Function":
                    {
                        Dictionary<string, object> result = [];
                        result.Add("type", "Function");

                        if (value is not IDictionary<string, object> valueDict)
                        {
                            throw new Exception("todo");
                        }
                        result.Add("functionType", CadenceTypeParser.ParseFlowType(valueDict["functionType"]));
                        return result;
                    }
                case "Void":
                    {
                        return "Void";
                    }
                case "Path":
                    {
                        return ParsePathType(value);
                    }
                case "Capability":
                    {
                        if (value is not IDictionary<string, object> valueDict)
                        {
                            throw new Exception("todo");
                        }

                        return new Dictionary<string, object>
                        {
                            { "path", ParsePathType(valueDict["path"]) },
                            { "address", valueDict["address"] }, //str
                            { "borrowType", CadenceTypeParser.ParseFlowType(valueDict["borrowType"]) }
                        };
                    }
                default: //primitive json, string bool number etc
                    {
                        return value;
                    }
            }
        }

        private static (string name, object value) ParseEventField(IDictionary<string, object> field)
        {
            return (field["name"].ToString().ToCamelCase(), ParseVerboseType(field["value"]));
        }

        private static IDictionary<string, object> ParsePathType(object value)
        {
            if (value is not IDictionary<string, object> valueDict)
            {
                throw new Exception("todo");
            }

            return new Dictionary<string, object>()
            {
                { "domain", valueDict["domain"] },
                { "identifer", valueDict["identifer"] }
            };
        }
    }
}
