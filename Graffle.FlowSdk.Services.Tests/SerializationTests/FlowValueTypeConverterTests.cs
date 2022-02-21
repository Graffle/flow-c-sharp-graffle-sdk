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
        }

        private Utf8JsonReader CreateJsonReader(string json) => new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
    }
}