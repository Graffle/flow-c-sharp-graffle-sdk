using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Graffle.FlowSdk.Services.Serialization
{
    public class JsonCadenceInterchangeFormatParser
    {
        private static readonly Newtonsoft.Json.JsonConverter _expando = new ExpandoObjectConverter();

        public static object FromJson(string json)
        {
            var parsed = JsonConvert.DeserializeObject<ExpandoObject>(json, _expando);

            return ParseFieldValue(parsed);
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

            var fields = valueDict["fields"] as IList<object>;

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
                res.Data[parsedField.name.ToCamelCase()] = parsedField.value;
            }

            return res;
        }

        public static object ParseFieldValue(object field)
        {
            if (field is not IDictionary<string, object> fieldValue)
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
                            var innerValue = ParseFieldValue(fieldObj["value"]);

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

                            var parsedKey = ParseFieldValue(innerObj["key"]);
                            var parsedValue = ParseFieldValue(innerObj["value"]);

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
                            values.Add(ParseFieldValue(arrValue));
                        }

                        return values;
                    }
                case "Optional":
                    {
                        if (value is null)
                            return null;

                        return ParseFieldValue(value);
                    }
                default: //primitive
                    {
                        return value;
                    }
            }
        }

        public static (string name, object value) ParseEventField(IDictionary<string, object> field)
        {
            var res = new Dictionary<string, object>();
            var name = field["name"].ToString();

            if (field["value"] is not IDictionary<string, object> valueObj)
                throw new Exception("todo");

            return (name.ToCamelCase(), ParseFieldValue(valueObj));
        }
    }
}