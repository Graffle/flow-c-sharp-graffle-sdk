using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Graffle.FlowSdk.Services.Serialization;
using System.Runtime.CompilerServices;
using Org.BouncyCastle.Asn1.Cmp;
using Polly.Retry;
using Org.BouncyCastle.Security;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Org.BouncyCastle.Asn1.Misc;
using System.Text.Json;
using Graffle.FlowSdk.Types;
using Graffle.FlowSdk.Services.Extensions;

namespace Graffle.FlowSdk.Services.Tests.CadenceJsonTests
{
    [TestClass]
    public class CadenceJsonInterpreterTests
    {
        private void JsonEquals(object first, object second)
        {
            var firstJson = JsonSerializer.Serialize(first);
            var secondJson = JsonSerializer.Serialize(second);

            Assert.AreEqual(firstJson, secondJson);
        }

        [TestMethod]
        public void AddressType()
        {
            var json = @"{""type"":""Address"",""value"":""0x66d6f450e25a4e22""}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            Assert.IsInstanceOfType<string>(res);
            Assert.AreEqual("0x66d6f450e25a4e22", res);

            var fvt = FlowValueType.CreateFromCadence(json);
            var obj = FlowValueTypeUtility.FlowTypeToPrimitive(fvt);
            JsonEquals(res, obj);
        }

        [TestMethod]
        public void ArrayType()
        {
            var json = @"{""type"":""Array"",""value"": [ {""type"":""Int16"", ""value"":""123""}, {""type"":""String"",""value"":""hello world""}]}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            if (res is not List<object> values)
            {
                Assert.Fail("res not List<object>");
                return; //make compiler happy
            }

            //first item
            var first = values[0];
            Assert.AreEqual((long)123, first);

            //second item
            var second = values[1];
            Assert.AreEqual("hello world", second);

            var fvt = FlowValueType.CreateFromCadence(json);
            var obj = FlowValueTypeUtility.FlowTypeToPrimitive(fvt);
            JsonEquals(res, obj);
        }

        [TestMethod]
        [DataRow("Struct")]
        [DataRow("Resource")]
        [DataRow("Contract")]
        [DataRow("Event")]
        [DataRow("Enum")]
        public void ArrayType_NestedCompositeType(string nestedCompositeType)
        {
            /*
                        {
                            "type":"Array",
                            "value":[
                                {
                                    "type":"Struct",
                                    "value": {
                                        "id":"structId",
                                        "fields": [
                                            {
                                                "name":"structField1",
                                                "value": {
                                                    "type":"Int",
                                                    "value": "2"
                                                }
                                            }
                                        ]
                                    }
                                }
                            ]
                        }
                        */

            var json = $"{{\"type\":\"Array\",\"value\":[{{\"type\":\"{nestedCompositeType}\",\"value\":{{\"id\":\"structId\",\"fields\":[{{\"name\":\"structField1\",\"value\":{{\"type\":\"Int\",\"value\":\"2\"}}}}]}}}}]}}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            if (res is not List<object> arr)
            {
                Assert.Fail("res not List<object>");
                return; //make compiler happy
            }

            Assert.AreEqual(1, arr.Count);

            var composite = arr[0] as IDictionary<string, object>;
            if (composite == null)
            {
                Assert.Fail("Expected dictionary");
                return;
            }

            Assert.AreEqual(1, composite.Count());

            var kvp = composite.First();
            Assert.AreEqual("structField1", kvp.Key);
            Assert.AreEqual(2, kvp.Value);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void BoolType(bool value)
        {
            var json = $"{{\"type\":\"Bool\",\"value\":{value.ToString().ToLower()}}}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            Assert.IsInstanceOfType<bool>(res);
            Assert.AreEqual(value, res);
        }

        [TestMethod]
        public void CapabilityType_OldJson()
        {
            var json = @"{""type"":""Capability"",""value"":{""path"":""/public/someInteger"",""address"":""0x1"",""borrowType"":""Int""}}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            if (res is not IDictionary<string, object> dict)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.IsTrue(dict.ContainsKey("path"));
            Assert.IsTrue(dict.ContainsKey("address"));
            Assert.IsTrue(dict.ContainsKey("borrowType"));

            Assert.AreEqual("/public/someInteger", dict["path"]);
            Assert.AreEqual("0x1", dict["address"]);
            Assert.AreEqual("Int", dict["borrowType"]);
        }

        [TestMethod]
        public void CapabilityType_NewJson()
        {
            /*
            {
                "type": "Capability",
                "value": {
                    "path": {
                        "type": "Path",
                        "value": {
                            "domain": "public",
                            "identifier": "someInteger"
                        }
                    },
                    "address": "0x1",
                    "borrowType": {
                        "kind": "Int"
                    }
                }
            }
            */

            var json = "{\"type\":\"Capability\",\"value\":{\"path\":{\"type\":\"Path\",\"value\":{\"domain\":\"public\",\"identifier\":\"someInteger\"}},\"address\":\"0x1\",\"borrowType\":{\"kind\":\"Int\"}}}";

            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);
            if (res is not IDictionary<string, object> dict)
            {
                Assert.Fail("Expected dictionary");
                return;
            }

            Assert.AreEqual(3, dict.Count);
            Assert.IsTrue(dict.ContainsKey("path"));
            Assert.IsTrue(dict.ContainsKey("address"));
            Assert.IsTrue(dict.ContainsKey("borrowType"));

            if (dict["path"] is not IDictionary<string, object> path)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.IsTrue(path.ContainsKey("domain"));
            Assert.IsTrue(path.ContainsKey("identifier"));
            Assert.AreEqual("public", path["domain"]);
            Assert.AreEqual("someInteger", path["identifier"]);

            Assert.AreEqual("0x1", dict["address"]);

            if (dict["borrowType"] is not IDictionary<string, object> borrowType)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.IsTrue(borrowType.ContainsKey("kind"));
            Assert.AreEqual("Int", borrowType["kind"]);
        }

        [TestMethod]
        [DataRow("Struct")]
        [DataRow("Resource")]
        [DataRow("Event")]
        [DataRow("Contract")]
        [DataRow("Enum")]
        public void CompositeType(string type)
        {
            var json = $"{{\"type\":\"{type}\",\"value\":{{\"fields\":[{{\"name\":\"intField\",\"value\":{{\"type\":\"Int16\",\"value\":\"123\"}}}},{{\"name\":\"stringField\",\"value\":{{\"type\":\"String\",\"value\":\"hello\"}}}}],\"id\":\"idString\"}}}}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            if (res is not IDictionary<string, object> dict)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.AreEqual(2, dict.Count);
            Assert.IsTrue(dict.ContainsKey("intField"));
            Assert.IsTrue(dict.ContainsKey("stringField"));
            Assert.AreEqual((long)123, dict["intField"]);
            Assert.AreEqual("hello", dict["stringField"]);
        }

        [TestMethod]
        public void DictionaryType()
        {
            var json = @"{""type"":""Dictionary"",""value"":[{""key"":{""type"":""UInt8"",""value"":""123""},""value"":{""type"":""String"",""value"":""test""}}]}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            if (res is not IDictionary<string, object> dict)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.IsTrue(dict.ContainsKey("123"));
            Assert.AreEqual("test", dict["123"]);
        }

        [TestMethod]
        public void Fix64()
        {
            var json = @"{""type"":""Fix64"",""value"":""-100.002""}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            Assert.AreEqual(-100.002m, res);
        }

        [TestMethod]
        public void UFix64()
        {
            var json = @"{""type"":""UFix64"",""value"":""100.002""}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            Assert.AreEqual(100.002m, res);
        }

        [TestMethod]
        [DataRow("Resource")]
        [DataRow("Struct")]
        [DataRow("Event")]
        [DataRow("Contract")]
        [DataRow("StructInterface")]
        [DataRow("ResourceInterface")]
        [DataRow("ContractInterface")]
        public void FlowCompositeType(string kind)
        {
            var json = $"{{\"type\":\"Type\",\"value\":{{\"staticType\":{{\"kind\":\"{kind}\",\"typeID\":\"A.ff68241f0f4fd521.DrSeuss.NFT\",\"fields\":[{{\"id\":\"uuid\",\"type\":{{\"kind\":\"UInt64\"}}}},{{\"id\":\"id\",\"type\":{{\"kind\":\"UInt64\"}}}},{{\"id\":\"mintNumber\",\"type\":{{\"kind\":\"UInt32\"}}}},{{\"id\":\"contentCapability\",\"type\":{{\"kind\":\"Capability\",\"type\":\"\"}}}},{{\"id\":\"contentId\",\"type\":{{\"kind\":\"String\"}}}}],\"initializers\":[],\"type\":\"\"}}}}}}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            if (res is not IDictionary<string, object> dict)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.IsTrue(dict.ContainsKey("kind"));
            Assert.IsTrue(dict.ContainsKey("type"));
            Assert.IsTrue(dict.ContainsKey("typeID"));
            Assert.IsTrue(dict.ContainsKey("initializers"));
            Assert.IsTrue(dict.ContainsKey("fields"));

            Assert.AreEqual(kind, dict["kind"]);
            Assert.AreEqual(string.Empty, dict["type"]);
            Assert.AreEqual("A.ff68241f0f4fd521.DrSeuss.NFT", dict["typeID"]);

            if (dict["fields"] is not IList<object> fields)
            {
                Assert.Fail("expected list");
                return;
            }

            Assert.AreEqual(5, fields.Count);

            if (dict["initializers"] is not IList<object> initializers)
            {
                Assert.Fail("expected list");
                return;
            }

            Assert.AreEqual(0, initializers.Count);
        }

        [TestMethod]
        public void FlowCompositeType_WithInitializers()
        {
            var json = "{\"type\":\"Type\",\"value\":{\"staticType\":{\"kind\":\"Resource\",\"type\":\"\",\"typeID\":\"0x3.GreatContract.GreatNFT\",\"initializers\":[[{\"label\":\"foo\",\"id\":\"bar\",\"type\":{\"kind\":\"String\"}}]],\"fields\":[{\"id\":\"foo\",\"type\":{\"kind\":\"String\"}}]}}}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            if (res is not IDictionary<string, object> dict)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.IsTrue(dict.ContainsKey("kind"));
            Assert.IsTrue(dict.ContainsKey("type"));
            Assert.IsTrue(dict.ContainsKey("typeID"));
            Assert.IsTrue(dict.ContainsKey("initializers"));
            Assert.IsTrue(dict.ContainsKey("fields"));

            Assert.AreEqual("Resource", dict["kind"]);
            Assert.AreEqual(string.Empty, dict["type"]);
            Assert.AreEqual("0x3.GreatContract.GreatNFT", dict["typeID"]);

            if (dict["fields"] is not IList<object> fields)
            {
                Assert.Fail("expected list");
                return;
            }

            Assert.AreEqual(1, fields.Count);

            if (dict["initializers"] is not IList<object> initializers)
            {
                Assert.Fail("expected list");
                return;
            }

            Assert.AreEqual(1, initializers.Count);

            if (initializers.First() is not IDictionary<string, object> init)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.IsTrue(init.ContainsKey("label"));
            Assert.IsTrue(init.ContainsKey("id"));
            Assert.IsTrue(init.ContainsKey("type"));

            Assert.AreEqual("foo", init["label"]);
            Assert.AreEqual("bar", init["id"]);

            if (init["type"] is not IDictionary<string, object> initType)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.IsTrue(initType.ContainsKey("kind"));
            Assert.AreEqual("String", initType["kind"]);
        }

        [TestMethod]
        public void FunctionType()
        {
            var json = @"{""type"":""Function"",""value"":{""functionType"":{""kind"":""Function"",""typeID"":""(():Void)"",""parameters"":[],""return"":{""kind"":""Void""}}}}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            if (res is not IDictionary<string, object> dict)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.AreEqual(1, dict.Count);
            Assert.IsTrue(dict.ContainsKey("functionType"));

            if (dict["functionType"] is not IDictionary<string, object> functionType)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.IsTrue(functionType.ContainsKey("kind"));
            Assert.IsTrue(functionType.ContainsKey("typeID"));
            Assert.IsTrue(functionType.ContainsKey("parameters"));
            Assert.IsTrue(functionType.ContainsKey("return"));

            Assert.AreEqual("Function", functionType["kind"]);
            Assert.AreEqual("(():Void)", functionType["typeID"]);

            if (functionType["parameters"] is not List<object> funcParams)
            {
                Assert.Fail("expected list");
                return;
            }

            Assert.AreEqual(0, funcParams.Count);

            if (functionType["return"] is not IDictionary<string, object> returnType)
            {
                Assert.Fail("expected dictionary");
                return;
            }
            Assert.IsTrue(returnType.ContainsKey("kind"));
            Assert.AreEqual("Void", returnType["kind"]);
        }

        [TestMethod]
        public void Int8()
        {
            var json = @"{""type"":""Int8"",""value"":""0""}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            Assert.AreEqual(0L, res);
        }

        [TestMethod]
        public void Int16()
        {
            var json = @"{""type"":""Int16"",""value"":""0""}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            Assert.AreEqual(0L, res);
        }

        [TestMethod]
        public void Int32()
        {
            var json = @"{""type"":""Int32"",""value"":""0""}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            Assert.AreEqual(0L, res);
        }

        [TestMethod]
        public void Int64()
        {
            var json = @"{""type"":""Int64"",""value"":""0""}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            Assert.AreEqual(0L, res);
        }

        [TestMethod]
        public void Int128()
        {
            var json = @"{""type"":""Int128"",""value"":""91389681247993671255432112000000000""}";

            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);
            Assert.AreEqual("91389681247993671255432112000000000", res);
        }

        [TestMethod]
        public void Int256()
        {
            var json = @"{""type"":""Int256"",""value"":""91389681247993671255432112000000000""}";

            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);
            Assert.AreEqual("91389681247993671255432112000000000", res);
        }

        [TestMethod]
        public void IntType()
        {
            var json = @"{""type"":""Int"",""value"":""0""}";

            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);
            Assert.AreEqual(0, res);
        }

        [TestMethod]
        public void IntType_BigNumber()
        {
            var json = @"{""type"":""Int"",""value"":""91389681247993671255432112000000000""}";

            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);
            Assert.AreEqual("91389681247993671255432112000000000", res);
        }

        [TestMethod]
        public void OptionalType_Null()
        {
            var json = "{\"type\":\"Optional\",\"value\":null}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);
            Assert.IsNull(res);
        }

        [TestMethod]
        public void Optional_StringType()
        {
            var json = "{\"type\":\"Optional\",\"value\":{\"type\":\"String\",\"value\":\"Derrick \\\"The Black Beast\\\" Lewis delivers a lights out uppercut against Curtis Blaydes, to break the record for the most KOs in UFC heavyweight history and tie for the most KO/TKO wins in UFC history\"}}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            Assert.AreEqual("Derrick \"The Black Beast\" Lewis delivers a lights out uppercut against Curtis Blaydes, to break the record for the most KOs in UFC heavyweight history and tie for the most KO/TKO wins in UFC history", res);
        }

        [TestMethod]
        public void PathType()
        {
            var json = @"{""type"":""Path"",""value"":{""domain"":""testDomain"",""identifier"":""testIdentifier""}}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            if (res is not IDictionary<string, object> dict)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.IsTrue(dict.ContainsKey("domain"));
            Assert.IsTrue(dict.ContainsKey("identifier"));
            Assert.AreEqual("testDomain", dict["domain"]);
            Assert.AreEqual("testIdentifier", dict["identifier"]);
        }

        [TestMethod]
        public void StringType()
        {
            var json = "{\"type\":\"String\",\"value\":\"Appearance: The xG Reward for players with game time in a fixture.\n\nGet xG Rewards for your football achievements.\nBuild your collection - your story.\nUnlock xG experiences.\n\nhttps://linktr.ee/xgstudios\"}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            Assert.AreEqual("Appearance: The xG Reward for players with game time in a fixture.\n\nGet xG Rewards for your football achievements.\nBuild your collection - your story.\nUnlock xG experiences.\n\nhttps://linktr.ee/xgstudios", res);
        }

        [TestMethod]
        public void UInt8()
        {
            var json = @"{""type"":""UInt8"",""value"":""0""}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            Assert.AreEqual(0ul, res);
        }

        [TestMethod]
        public void UInt16()
        {
            var json = @"{""type"":""UInt16"",""value"":""0""}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            Assert.AreEqual(0ul, res);
        }

        [TestMethod]
        public void UInt32()
        {
            var json = @"{""type"":""UInt32"",""value"":""0""}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            Assert.AreEqual(0ul, res);
        }

        [TestMethod]
        public void UInt64()
        {
            var json = @"{""type"":""UInt64"",""value"":""0""}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            Assert.AreEqual(0ul, res);
        }

        [TestMethod]
        public void UInt128()
        {
            var json = @"{""type"":""UInt128"",""value"":""0""}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            Assert.AreEqual("0", res);
        }

        [TestMethod]
        public void UInt256()
        {
            var json = @"{""type"":""UInt256"",""value"":""0""}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            Assert.AreEqual("0", res);
        }

        [TestMethod]
        public void UInt()
        {
            var json = @"{""type"":""UInt"",""value"":""0""}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            Assert.AreEqual(0U, res);
        }

        [TestMethod]
        public void Word8()
        {
            var json = @"{""type"":""Word8"",""value"":""100""}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            Assert.AreEqual(100ul, res);
        }

        [TestMethod]
        public void Word16()
        {
            var json = @"{""type"":""Word16"",""value"":""100""}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            Assert.AreEqual(100ul, res);
        }

        [TestMethod]
        public void Word32()
        {
            var json = @"{""type"":""Word32"",""value"":""100""}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            Assert.AreEqual(100ul, res);
        }

        [TestMethod]
        public void Word64()
        {
            var json = @"{""type"":""Word32"",""value"":""100""}";
            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

            Assert.AreEqual(100ul, res);
        }

    }
}