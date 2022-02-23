using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Graffle.FlowSdk.Services.Tests.SerializationTests
{
    [TestClass]
    public class GraffleCompositeTypeConverterTests
    {
        [TestMethod]
        public void DeserializeFlowCadence_PrimitiveType_ReturnsGraffleCompositeType()
        {
            var cadenceJson = @"{""type"":""UInt32"",""value"":""112233""}";
            var uint32Field = CreateField("testName", cadenceJson);

            var fields = new List<Dictionary<string, string>>() { uint32Field };

            var converter = new GraffleCompositeTypeConverter();
            var result = converter.DeserializeFlowCadence("testId", "event", fields);

            Assert.IsNotNull(result);
            Assert.AreEqual("testId", result.Id);
            Assert.AreEqual("event", result.Type);

            var data = result.Data;
            Assert.AreEqual(1, data.Keys.Count);

            var key = data.Keys.First();
            Assert.AreEqual("testName", key);

            var value = data[key];

            Assert.AreEqual((uint)112233, value);
        }

        [TestMethod]
        public void DeserializeFlowCadence_PrimitiveType_Optional_ReturnsGraffleCompositeType()
        {
            var cadenceJson = $"{{\"type\":\"Optional\",\"value\":{{\"type\":\"Int16\",\"value\":123}}}}";
            var field = CreateField("testName", cadenceJson);

            var fields = new List<Dictionary<string, string>>() { field };

            var converter = new GraffleCompositeTypeConverter();
            var result = converter.DeserializeFlowCadence("testId", "event", fields);

            Assert.IsNotNull(result);
            Assert.AreEqual("testId", result.Id);
            Assert.AreEqual("event", result.Type);

            var data = result.Data;
            Assert.AreEqual(1, data.Keys.Count);

            var key = data.Keys.First();
            Assert.AreEqual("testName", key);

            var value = data[key];
            Assert.AreEqual(123, value);
        }

        [TestMethod]
        public void DeserializeFlowCadence_OptionalType_Null_ReturnsGraffleCompositeType()
        {
            var cadenceJson = "{\"type\":\"Optional\",\"value\":null}";
            var field = CreateField("testName", cadenceJson);

            var fields = new List<Dictionary<string, string>>() { field };

            var converter = new GraffleCompositeTypeConverter();
            var result = converter.DeserializeFlowCadence("testId", "event", fields);

            Assert.IsNotNull(result);
            Assert.AreEqual("testId", result.Id);
            Assert.AreEqual("event", result.Type);

            var data = result.Data;
            Assert.AreEqual(1, data.Keys.Count);

            var key = data.Keys.First();
            Assert.AreEqual("testName", key);

            var value = data[key];
            Assert.IsNull(value);
        }

        [TestMethod]
        public void DeserializeFlowCadence_DictionaryType_ReturnsGraffleComposite()
        {
            var cadenceJson = @"{""type"":""Dictionary"",""value"":[{""key"":{""type"":""Int32"",""value"":""123""},""value"":{""type"":""String"",""value"":""test""}}]}";
            var field = CreateField("testName", cadenceJson);

            var fields = new List<Dictionary<string, string>>() { field };

            var converter = new GraffleCompositeTypeConverter();
            var result = converter.DeserializeFlowCadence("testId", "event", fields);

            Assert.IsNotNull(result);
            Assert.AreEqual("testId", result.Id);
            Assert.AreEqual("event", result.Type);

            var data = result.Data;

            var key = data.Keys.First();
            Assert.AreEqual("testName", key);

            var value = data[key];
            Assert.IsInstanceOfType(value, typeof(Dictionary<object, object>));

            var dict = value as Dictionary<object, object>;

            //verify dictionary values
            Assert.AreEqual(1, dict.Keys.Count);

            var first = dict.Keys.First();
            Assert.AreEqual("123", first); //dictionary keys are string?
            Assert.AreEqual("test", dict[first]);
        }

        [TestMethod]
        public void DeserializeFlowCadence_ArrayType_PrimitiveValues_ReturnsGraffleCompositeType()
        {
            var cadenceJson = @"{""type"":""Array"",""value"": [ {""type"":""Int16"", ""value"":""123""}, {""type"":""String"",""value"":""hello world""}]}";
            var field = CreateField("testName", cadenceJson);

            var fields = new List<Dictionary<string, string>>() { field };

            var converter = new GraffleCompositeTypeConverter();
            var result = converter.DeserializeFlowCadence("testId", "event", fields);

            Assert.IsNotNull(result);
            Assert.AreEqual("testId", result.Id);
            Assert.AreEqual("event", result.Type);

            var data = result.Data;

            var key = data.Keys.First();
            Assert.AreEqual("testName", key);

            var value = data[key];
            Assert.IsInstanceOfType(value, typeof(List<object>));

            //verify array values
            var array = value as List<object>;
            Assert.AreEqual(2, array.Count);
            Assert.AreEqual((Int16)123, array[0]);
            Assert.AreEqual("hello world", array[1]);
        }

        [TestMethod]
        public void DeserializeFlowCadence_ArrayType_HasComplexType_ReturnsGraffleCompositeType()
        {
            /*
                {
                    "type":"Array",
                    "value": [
                            {
                                "type":"UInt32",
                                "value":"12345"
                            },
                            {
                                "type":"Struct",
                                "value": {
                                        "id":"structId",
                                        "fields":[
                                            {
                                                "name":"structField1",
                                                "value": {
                                                    "type":"String",
                                                    "value":"structStringValue"
                                                }
                                            },
                                            {
                                                "name":"structField2",
                                                "value": {
                                                    "type":"Int64",
                                                    "value":"5432"
                                                }
                                            }
                                        ]
                                    }
                            }
                    ]
                }
            */
            var cadenceJson = @"{""type"":""Array"",""value"":[{""type"":""UInt32"",""value"":""12345""},{""type"":""Struct"",""value"":{""id"":""structId"",""fields"":[{""name"":""structField1"",""value"":{""type"":""String"",""value"":""structStringValue""}},{""name"":""structField2"",""value"":{""type"":""Int64"",""value"":""5432""}}]}}]}";
            var field = CreateField("testName", cadenceJson);

            var fields = new List<Dictionary<string, string>>() { field };

            var converter = new GraffleCompositeTypeConverter();
            var result = converter.DeserializeFlowCadence("testId", "event", fields);

            Assert.IsNotNull(result);
            Assert.AreEqual("testId", result.Id);
            Assert.AreEqual("event", result.Type);

            var data = result.Data;

            var key = data.Keys.First();
            Assert.AreEqual("testName", key);

            var value = data[key];
            Assert.IsInstanceOfType(value, typeof(List<object>));

            //verify array values
            Assert.AreEqual(2, value.Count);
            Assert.AreEqual((uint)12345, value[0]);
            Assert.IsInstanceOfType(value[1], typeof(GraffleCompositeType));

            //verify struct
            var composite = value[1] as GraffleCompositeType;
            Assert.AreEqual("structId", composite.Id);
            Assert.AreEqual("Struct", composite.Type);

            //verify struct fields
            Assert.IsInstanceOfType(composite.Data, typeof(Dictionary<string, object>));

            var structFields = composite.Data as Dictionary<string, object>;
            Assert.AreEqual(2, structFields.Keys.Count);

            var first = structFields.Keys.First();
            Assert.AreEqual("structField1", first);
            Assert.AreEqual("structStringValue", structFields[first]);

            var second = structFields.Keys.Skip(1).First();
            Assert.AreEqual(second, "structField2");
            Assert.AreEqual((long)5432, structFields[second]);
        }

        [TestMethod]
        [DataRow("Struct")]
        [DataRow("Resource")]
        [DataRow("Event")]
        [DataRow("Contract")]
        [DataRow("Enum")]
        public void DeserializeFlowCadence_CompositeType_ReturnsGraffleCompositeType(string type)
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
            var cadenceJson = $"{{\"type\":\"{type}\",\"value\":{{\"fields\":[{{\"name\":\"intField\",\"value\":{{\"type\":\"Int16\",\"value\":\"123\"}}}},{{\"name\":\"stringField\",\"value\":{{\"type\":\"String\",\"value\":\"hello\"}}}}],\"id\":\"idString\"}}}}";
            var field = CreateField("testName", cadenceJson);

            var fields = new List<Dictionary<string, string>>() { field };

            var converter = new GraffleCompositeTypeConverter();
            var result = converter.DeserializeFlowCadence("testId", type, fields);

            Assert.IsNotNull(result);
            Assert.AreEqual("testId", result.Id);
            Assert.AreEqual(type, result.Type);

            var data = result.Data;

            var key = data.Keys.First();
            Assert.AreEqual("testName", key);

            //validate struct fields
            var value = data[key];
            Assert.IsInstanceOfType(value, typeof(Dictionary<string, object>));

            var structFields = value as Dictionary<string, object>;
            Assert.AreEqual(2, structFields.Keys.Count);

            var first = structFields.Keys.First();
            Assert.AreEqual(first, "intField");
            Assert.AreEqual((Int16)123, structFields[first]);

            var second = structFields.Keys.Skip(1).First();
            Assert.AreEqual("stringField", second);
            Assert.AreEqual("hello", structFields[second]);
        }

        [TestMethod]
        public void Read_ReturnsGraffleCompositeType()
        {
            var json = @"{""type"":""Event"",""value"":{""id"":""A.2a37a78609bba037.TheFabricantS1ItemNFT.ItemDataCreated"",""fields"":[{""name"":""itemDataID"",""value"":{""type"":""UInt32"",""value"":""152""}},{""name"":""coCreator"",""value"":{""type"":""Address"",""value"":""0x125d80d43ddbdb9a""}},{""name"":""metadatas"",""value"":{""type"":""Dictionary"",""value"":[{""key"":{""type"":""String"",""value"":""itemVideo""},""value"":{""type"":""Struct"",""value"":{""id"":""A.2a37a78609bba037.TheFabricantS1ItemNFT.Metadata"",""fields"":[{""name"":""metadataValue"",""value"":{""type"":""String"",""value"":""https://leela.mypinata.cloud/ipfs/QmeirDvh3TrDgtDdfvyjQXF87DusXksymtzmA4RABBVreo/LOOP.mp4""}},{""name"":""mutable"",""value"":{""type"":""Bool"",""value"":true}}]}}},{""key"":{""type"":""String"",""value"":""secondaryColor""},""value"":{""type"":""Struct"",""value"":{""id"":""A.2a37a78609bba037.TheFabricantS1ItemNFT.Metadata"",""fields"":[{""name"":""metadataValue"",""value"":{""type"":""String"",""value"":""545454""}},{""name"":""mutable"",""value"":{""type"":""Bool"",""value"":false}}]}}},{""key"":{""type"":""String"",""value"":""primaryColor""},""value"":{""type"":""Struct"",""value"":{""id"":""A.2a37a78609bba037.TheFabricantS1ItemNFT.Metadata"",""fields"":[{""name"":""metadataValue"",""value"":{""type"":""String"",""value"":""FF803E""}},{""name"":""mutable"",""value"":{""type"":""Bool"",""value"":false}}]}}},{""key"":{""type"":""String"",""value"":""itemImage4""},""value"":{""type"":""Struct"",""value"":{""id"":""A.2a37a78609bba037.TheFabricantS1ItemNFT.Metadata"",""fields"":[{""name"":""metadataValue"",""value"":{""type"":""String"",""value"":""""}},{""name"":""mutable"",""value"":{""type"":""Bool"",""value"":true}}]}}},{""key"":{""type"":""String"",""value"":""itemImage3""},""value"":{""type"":""Struct"",""value"":{""id"":""A.2a37a78609bba037.TheFabricantS1ItemNFT.Metadata"",""fields"":[{""name"":""metadataValue"",""value"":{""type"":""String"",""value"":""""}},{""name"":""mutable"",""value"":{""type"":""Bool"",""value"":true}}]}}},{""key"":{""type"":""String"",""value"":""itemImage2""},""value"":{""type"":""Struct"",""value"":{""id"":""A.2a37a78609bba037.TheFabricantS1ItemNFT.Metadata"",""fields"":[{""name"":""metadataValue"",""value"":{""type"":""String"",""value"":""""}},{""name"":""mutable"",""value"":{""type"":""Bool"",""value"":true}}]}}},{""key"":{""type"":""String"",""value"":""season""},""value"":{""type"":""Struct"",""value"":{""id"":""A.2a37a78609bba037.TheFabricantS1ItemNFT.Metadata"",""fields"":[{""name"":""metadataValue"",""value"":{""type"":""String"",""value"":""1""}},{""name"":""mutable"",""value"":{""type"":""Bool"",""value"":false}}]}}},{""key"":{""type"":""String"",""value"":""itemImage""},""value"":{""type"":""Struct"",""value"":{""id"":""A.2a37a78609bba037.TheFabricantS1ItemNFT.Metadata"",""fields"":[{""name"":""metadataValue"",""value"":{""type"":""String"",""value"":""https://leela.mypinata.cloud/ipfs/QmeirDvh3TrDgtDdfvyjQXF87DusXksymtzmA4RABBVreo/LOOP_poster.png""}},{""name"":""mutable"",""value"":{""type"":""Bool"",""value"":true}}]}}}]}}]}}";
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            var reader = new Utf8JsonReader(bytes);
            var converter = new GraffleCompositeTypeConverter();
            var result = converter.Read(ref reader, null, null);

            Assert.IsNotNull(result);
            Assert.AreEqual("Event", result.Type);
            Assert.AreEqual("A.2a37a78609bba037.TheFabricantS1ItemNFT.ItemDataCreated", result.Id);

            var data = result.Data;
            Assert.IsNotNull(data);

            //test some of the data
            var dict = data["metadatas"];
            Assert.AreEqual(8, dict.Count);

            //lets check out a single struct in here
            var flowStruct = dict["itemVideo"];
            Assert.IsInstanceOfType(flowStruct, typeof(GraffleCompositeType));
            var composite = flowStruct as GraffleCompositeType;
            Assert.AreEqual("A.2a37a78609bba037.TheFabricantS1ItemNFT.Metadata", composite.Id);
            Assert.AreEqual("Struct", composite.Type);
            Assert.AreEqual(2, composite.Data.Count);
        }

        [TestMethod]
        public void Write_ThrowsNotImplementedException()
        {
            var converter = new GraffleCompositeTypeConverter();
            Assert.ThrowsException<NotImplementedException>(() => converter.Write(null, null, null));
        }

        private Dictionary<string, string> CreateField(string name, string cadenceJson)
        {
            return new Dictionary<string, string>()
            {
                { "name", name},
                { "value", cadenceJson }
            };
        }
    }
}