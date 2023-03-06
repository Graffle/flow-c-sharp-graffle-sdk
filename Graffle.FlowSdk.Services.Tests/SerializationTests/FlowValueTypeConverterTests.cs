using Graffle.FlowSdk.Types;
using Graffle.FlowSdk.Types.TypeDefinitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;

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
        [DataRow("Resource")]
        [DataRow("Event")]
        [DataRow("Contract")]
        [DataRow("Enum")]
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
            Assert.IsInstanceOfType(result, typeof(CompositeType));

            var composite = result as CompositeType;
            Assert.AreEqual(type, composite.Type);
            Assert.AreEqual("idString", composite.Id);

            Assert.AreEqual(2, composite.Fields.Count);

            var intField = composite.Fields.First(x => x.Name == "intField");
            Assert.AreEqual("Int16", intField.Value.Type);
            Assert.IsInstanceOfType(intField.Value, typeof(Int16Type));
            Assert.AreEqual((short)123, ((Int16Type)intField.Value).Data);

            var strField = composite.Fields.First(x => x.Name == "stringField");
            Assert.AreEqual("String", strField.Value.Type);
            Assert.IsInstanceOfType(strField.Value, typeof(StringType));
            Assert.AreEqual("hello", ((StringType)strField.Value).Data);
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
            Assert.IsInstanceOfType(result, typeof(CompositeType));

            var composite = result as CompositeType;
            var data = composite.Data;

            Assert.AreEqual(1, data.Fields.Count);
            Assert.AreEqual("structId", composite.Id);

            var item = data.Fields.First();
            Assert.AreEqual("arrayField", item.Name);
            Assert.IsInstanceOfType(item.Value, typeof(ArrayType));

            var arr = item.Value as ArrayType;
            Assert.AreEqual(2, arr.Data.Count);

            //validate array values
            var first = arr.Data.First();
            Assert.IsInstanceOfType(first, typeof(StringType));
            Assert.AreEqual("stringValue", ((StringType)first).Data);

            var second = arr.Data.Skip(1).First();
            Assert.IsInstanceOfType(second, typeof(Int64Type));
            Assert.AreEqual(1234567L, ((Int64Type)second).Data);
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
            Assert.IsInstanceOfType(result, typeof(CompositeType));

            var composite = result as CompositeType;
            Assert.AreEqual("structId", composite.Id);

            var fields = composite.Fields;
            Assert.AreEqual(1, fields.Count);

            //get the dictionary
            var item = fields.First();
            Assert.AreEqual("dictionaryField", item.Name);
            Assert.IsInstanceOfType(item.Value, typeof(DictionaryType));

            var dict = item.Value as DictionaryType;

            //validate dictionary values
            Assert.AreEqual(2, dict.Data.Keys.Count);

            var first = dict.Data.First();
            Assert.IsInstanceOfType(first.Key, typeof(IntType)); //these always come out as strings
            Assert.AreEqual(123, Cast<IntType>(first.Key).Data);
            Assert.IsInstanceOfType(first.Value, typeof(StringType));
            Assert.AreEqual("heyyyy", Cast<StringType>(first.Value).Data);

            var second = dict.Data.Skip(1).First();
            Assert.IsInstanceOfType(second.Key, typeof(AddressType));
            Assert.AreEqual("0x4", Cast<AddressType>(second.Key).Data);
            Assert.IsInstanceOfType(second.Value, typeof(Int32Type));
            Assert.AreEqual(-15, Cast<Int32Type>(second.Value).Data);
        }

        [TestMethod]
        [DataRow("UInt8")]
        [DataRow("Address")]
        [DataRow("String")]
        public void Read_FlowType(string kind)
        {
            var json = $"{{\"type\":\"Type\",\"staticType\":{{\"kind\":\"{kind}\"}}}}";
            var reader = CreateJsonReader(json);

            var converter = new FlowValueTypeConverter();
            var result = converter.Read(ref reader, null, null);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FlowType));

            var flowType = result as FlowType;
            Assert.AreEqual("Type", flowType.Type);
            Assert.IsInstanceOfType(flowType.Data, typeof(SimpleTypeDefinition));

            var simp = flowType.Data as SimpleTypeDefinition;
            Assert.AreEqual(kind, simp.Kind);
        }

        private T Cast<T>(FlowValueType flowType) where T : FlowValueType
        {
            return (T)flowType;
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