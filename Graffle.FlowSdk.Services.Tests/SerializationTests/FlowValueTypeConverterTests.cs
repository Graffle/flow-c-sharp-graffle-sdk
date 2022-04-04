using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text;
using Graffle.FlowSdk.Types;

namespace Graffle.FlowSdk.Services.Tests.SerializationTests
{
    [TestClass]
    public class FlowValueTypeConverterTests
    {
        [TestMethod]
        public void Read_PrimitiveType_ReturnsFlowValueType()
        {
            var json = @"{""type"":""Int16"",""value"":""123""}";
            var reader = CreateJsonReader(json);

            var converter = new FlowValueTypeConverter();
            var result = converter.Read(ref reader, null, null);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Int16Type));
            var data = (result as Int16Type).Data;
            Assert.AreEqual((short)123, data);
        }

        [TestMethod]
        public void Read_ArrayType_ReturnsFlowValueType()
        {
            var json = @"{""type"":""Array"",""value"":[{""type"":""Int16"",""value"":""123""},{""type"":""String"",""value"":""hello world""}]}";
            var reader = CreateJsonReader(json);

            var converter = new FlowValueTypeConverter();
            var result = converter.Read(ref reader, null, null);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ArrayType));

            var data = (result as ArrayType).Data;

            //validate array data
            Assert.AreEqual(2, data.Count);
            var first = data.First();
            Assert.IsInstanceOfType(first, typeof(Int16Type));
            var firstData = (first as Int16Type).Data;
            Assert.AreEqual((short)123, firstData);

            var second = data.Skip(1).First();
            Assert.IsInstanceOfType(second, typeof(StringType));
            var secondData = (second as StringType).Data;
            Assert.AreEqual("hello world", secondData);
        }

        [TestMethod]
        public void Read_DictionaryType_ReturnsFlowValueType()
        {
            var json = @"{""type"":""Dictionary"",""value"":[{""key"":{""type"":""UInt8"",""value"":""123""},""value"":{""type"":""String"",""value"":""test""}}]}";
            var reader = CreateJsonReader(json);

            var converter = new FlowValueTypeConverter();
            var result = converter.Read(ref reader, null, null);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DictionaryType));

            var data = (result as DictionaryType).Data;
            Assert.AreEqual(1, data.Keys.Count);

            var kvp = data.First();
            var key = kvp.Key;
            Assert.IsInstanceOfType(key, typeof(UInt8Type));
            Assert.AreEqual((uint)123, (key as UInt8Type).Data);

            var value = kvp.Value;
            Assert.IsInstanceOfType(value, typeof(StringType));
            Assert.AreEqual("test", (value as StringType).Data);
        }

        [TestMethod]
        [DataRow("Struct")]
        [DataRow("Event")]
        public void Read_ComplexType_ReturnsFlowValueType(string type)
        {
            /*
                {
                    "type":"{type}",
                    "value":{
                        "fields":[
                            {
                                "name":"intField",
                                "value":{
                                    "type":"Int16",
                                    "value":"123"
                                }
                            },
                            {
                                "name":"stringField",
                                "value":{
                                    "type":"String",
                                    "value":"hello"
                                }
                            }
                        ],
                        "id":"idString"
                    }
                }
            */
            var json = $"{{\"type\":\"{type}\",\"value\":{{\"fields\":[{{\"name\":\"intField\",\"value\":{{\"type\":\"Int16\",\"value\":\"123\"}}}},{{\"name\":\"stringField\",\"value\":{{\"type\":\"String\",\"value\":\"hello\"}}}}],\"id\":\"idString\"}}}}";
            var reader = CreateJsonReader(json);

            var converter = new FlowValueTypeConverter();
            var result = converter.Read(ref reader, null, null);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(GraffleCompositeType));

            var composite = result as GraffleCompositeType;
            var data = composite.Data;
            Assert.AreEqual(2, data.Keys.Count);

            //validate data
            var first = data.First();
            Assert.AreEqual("intField", first.Key);
            Assert.IsInstanceOfType(first.Value, typeof(Int16));
            Assert.AreEqual((Int16)123, first.Value);

            var second = data.Skip(1).First();
            Assert.AreEqual("stringField", second.Key);
            Assert.IsInstanceOfType(second.Value, typeof(string));
            Assert.AreEqual("hello", second.Value);
        }

        [TestMethod]
        public void Read_ComplexType_ContainsArray_ReturnsFlowValueType()
        {
            /*
            {
                "type":"Struct",
                "value": {
                    "id":"structId",
                    "fields": [
                        {
                            "name":"arrayField",
                            "value": {
                                "type":"Array",
                                "value": [
                                    {
                                        "type":"String",
                                        "value":"stringValue"
                                    },
                                    {
                                        "type":"Int64",
                                        "value":"1234567"
                                    }
                                ]
                            }
                        }
                    ]
                }
            }
            */
            var json = @"{""type"":""Struct"",""value"":{""id"":""structId"",""fields"":[{""name"":""arrayField"",""value"":{""type"":""Array"",""value"":[{""type"":""String"",""value"":""stringValue""},{""type"":""Int64"",""value"":""1234567""}]}}]}}";
            var reader = CreateJsonReader(json);

            var converter = new FlowValueTypeConverter();
            var result = converter.Read(ref reader, null, null);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(GraffleCompositeType));

            var composite = result as GraffleCompositeType;
            var data = composite.Data;

            Assert.AreEqual(1, data.Keys.Count);
            Assert.AreEqual("structId", composite.Id);

            var item = data.First();
            Assert.AreEqual("arrayField", item.Key);
            Assert.IsInstanceOfType(item.Value, typeof(List<object>));

            var arr = item.Value as List<object>;
            Assert.AreEqual(2, arr.Count);

            //validate array values
            Assert.IsInstanceOfType(arr[0], typeof(string));
            Assert.AreEqual("stringValue", arr[0]);

            Assert.IsInstanceOfType(arr[1], typeof(Int64));
            Assert.AreEqual((Int64)1234567, arr[1]);
        }

        [TestMethod]
        public void Read_ComplexType_ContainsDictionary_ReturnsFlowValueType()
        {
            /*
            {
                "type":"Struct",
                "value": {
                    "id":"structId",
                    "fields": [
                        {
                            "name":"dictionaryField",
                            "value": {
                                "type":"Dictionary",
                                "value": [
                                    {
                                        "key": {
                                            "type":"Int",
                                            "value":"123"
                                        },
                                        "value":{
                                            "type":"String",
                                            "value":"heyyyy"
                                        }
                                    },
                                    {
                                        "key": {
                                            "type":"Address",
                                            "value":"0x4"
                                        },
                                        "value":{
                                            "type":"Int32",
                                            "value":"-15"
                                        }
                                    }
                                ]
                            }
                        }
                    ]
                }
            }
            */
            string json = @"{""type"":""Struct"",""value"":{""id"":""structId"",""fields"":[{""name"":""dictionaryField"",""value"":{""type"":""Dictionary"",""value"":[{""key"":{""type"":""Int"",""value"":""123""},""value"":{""type"":""String"",""value"":""heyyyy""}},{""key"":{""type"":""Address"",""value"":""0x4""},""value"":{""type"":""Int32"",""value"":""-15""}}]}}]}}";
            var reader = CreateJsonReader(json);

            var converter = new FlowValueTypeConverter();
            var result = converter.Read(ref reader, null, null);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(GraffleCompositeType));

            var composite = result as GraffleCompositeType;
            Assert.AreEqual("structId", composite.Id);

            var data = composite.Data;
            Assert.AreEqual(1, data.Keys.Count);

            //get the dictionary
            var item = data.First();
            Assert.AreEqual("dictionaryField", item.Key);
            Assert.IsInstanceOfType(item.Value, typeof(Dictionary<object, object>));

            var dict = item.Value as Dictionary<object, object>;

            //validate dictionary values
            Assert.AreEqual(2, dict.Keys.Count);

            var first = dict.First();
            Assert.IsInstanceOfType(first.Key, typeof(string)); //these always come out as strings
            Assert.AreEqual("123", first.Key);
            Assert.IsInstanceOfType(first.Value, typeof(string));
            Assert.AreEqual("heyyyy", first.Value);

            var second = dict.Skip(1).First();
            Assert.IsInstanceOfType(second.Key, typeof(string));
            Assert.AreEqual("0x4", second.Key);
            Assert.IsInstanceOfType(second.Value, typeof(int));
            Assert.AreEqual(-15, second.Value);
        }

        [TestMethod]
        public void Write_ThrowsNotImplementedException()
        {
            var converter = new FlowValueTypeConverter();
            Assert.ThrowsException<NotImplementedException>(() => converter.Write(null, null, null));
        }

        private Utf8JsonReader CreateJsonReader(string json) => new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
    }
}