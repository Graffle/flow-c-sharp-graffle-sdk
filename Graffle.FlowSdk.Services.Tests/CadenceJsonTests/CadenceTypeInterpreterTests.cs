using Graffle.FlowSdk.Services.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Graffle.FlowSdk.Services.Tests.CadenceJsonTests
{
    [TestClass]
    public class CadenceTypeInterpreterTests
    {
        [TestMethod]
        public void Capability()
        {
            var json = "{\"kind\":\"Capability\",\"type\":{\"kind\":\"UInt8\"}}";

            var res = CadenceTypeInterpreter.ObjectFromCadenceJson(json);

            if (res is not IDictionary<string, object> dict)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.IsTrue(dict.ContainsKey("kind"));
            Assert.IsTrue(dict.ContainsKey("type"));

            if (dict["type"] is not IDictionary<string, object> typeDict)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.IsTrue(typeDict.ContainsKey("kind"));
            Assert.AreEqual(typeDict["kind"], "UInt8");
        }

        [TestMethod]
        public void Capability_NoType()
        {
            var json = "{\"kind\":\"Capability\",\"type\":null}";
            var res = CadenceTypeInterpreter.ObjectFromCadenceJson(json);

            if (res is not IDictionary<string, object> dict)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.IsTrue(dict.ContainsKey("kind"));
            Assert.IsTrue(dict.ContainsKey("type"));
            Assert.AreEqual(string.Empty, dict["type"]);
        }

        [TestMethod]
        public void CompositeType()
        {
            var json = "{\"kind\":\"Resource\",\"typeID\":\"compositeTypeId\",\"fields\":[{\"id\":\"fieldId\",\"type\":{\"kind\":\"Int32\"}}],\"initializers\":[],\"type\":\"\"}";
            var res = CadenceTypeInterpreter.ObjectFromCadenceJson(json);

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
            Assert.AreEqual("compositeTypeId", dict["typeID"]);

            if (dict["initializers"] is not List<object> inits)
            {
                Assert.Fail("expected list");
                return;
            }

            Assert.AreEqual(0, inits.Count);

            if (dict["fields"] is not List<object> fields)
            {
                Assert.Fail("expected list");
                return;
            }

            Assert.AreEqual(1, fields.Count);
        }

        [TestMethod]
        public void ConstantSizedArray()
        {
            var json = "{\"kind\":\"ConstantSizedArray\",\"size\":500,\"type\":{\"kind\":\"UInt8\"}}";
            var res = CadenceTypeInterpreter.ObjectFromCadenceJson(json);

            if (res is not IDictionary<string, object> dict)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.IsTrue(dict.ContainsKey("kind"));
            Assert.IsTrue(dict.ContainsKey("size"));
            Assert.IsTrue(dict.ContainsKey("type"));

            Assert.AreEqual("ConstantSizedArray", dict["kind"]);
            Assert.AreEqual(500L, dict["size"]);

            if (dict["type"] is not IDictionary<string, object> typeDict)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.AreEqual("UInt8", typeDict["kind"]);
        }

        [TestMethod]
        public void DictionaryTypeTest()
        {
            var json = "{\"kind\":\"Dictionary\",\"key\":{\"kind\":\"String\"},\"value\":{\"kind\":\"UInt8\"}}";
            var res = CadenceTypeInterpreter.ObjectFromCadenceJson(json);

            if (res is not IDictionary<string, object> dict)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.IsTrue(dict.ContainsKey("kind"));
            Assert.IsTrue(dict.ContainsKey("key"));
            Assert.IsTrue(dict.ContainsKey("value"));

            if (dict["key"] is not IDictionary<string, object> key)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.AreEqual("String", key["kind"]);

            if (dict["value"] is not IDictionary<string, object> value)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.AreEqual("UInt8", value["kind"]);
        }

        [TestMethod]
        public void EnumType()
        {
            var json = "{\"kind\":\"Enum\",\"type\":{\"kind\":\"String\"},\"typeID\":\"testTypeId\",\"initializers\":[],\"fields\":[{\"id\":\"fieldId\",\"type\":{\"kind\":\"UInt8\"}}]}";
            var res = CadenceTypeInterpreter.ObjectFromCadenceJson(json);

            if (res is not IDictionary<string, object> dict)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.IsTrue(dict.ContainsKey("type"));
            Assert.IsTrue(dict.ContainsKey("typeID"));
            Assert.IsTrue(dict.ContainsKey("fields"));
            Assert.IsTrue(dict.ContainsKey("initializers"));

            if (dict["type"] is not IDictionary<string, object> type)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.AreEqual("String", type["kind"]);

            Assert.AreEqual("testTypeId", dict["typeID"]);

            if (dict["initializers"] is not List<object> inits)
            {
                Assert.Fail("expected list");
                return;
            }

            Assert.AreEqual(0, inits.Count);

            if (dict["fields"] is not List<object> fields)
            {
                Assert.Fail("expected list");
                return;
            }

            Assert.AreEqual(1, fields.Count);
        }

        [TestMethod]
        public void InclusiveRangeType()
        {
            var json = "{\"kind\":\"InclusiveRange\",\"element\":{\"kind\":\"Int\"}}";
            var res = CadenceTypeInterpreter.ObjectFromCadenceJson(json);

            if (res is not IDictionary<string, object> dict)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.AreEqual("InclusiveRange", dict["kind"]);

            if (dict["element"] is not IDictionary<string, object> element)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.AreEqual("Int", element["kind"]);
        }

        [TestMethod]
        public void Reference_OldJson()
        {
            var json = "{\"kind\":\"Reference\",\"authorized\":true,\"type\":{\"kind\":\"String\"}}";
            var res = CadenceTypeInterpreter.ObjectFromCadenceJson(json);

            if (res is not IDictionary<string, object> reference)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.AreEqual("Reference", reference["kind"]);
            Assert.AreEqual(true, reference["authorized"]);

            if (reference["type"] is not IDictionary<string, object> type)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.AreEqual("String", type["kind"]);
        }

        [TestMethod]
        public void Reference_Cadence1_0_NullEntitlements()
        {
            var json = "{\"kind\":\"Reference\",\"authorization\":{\"kind\":\"Unauthorized\",\"entitlements\":null},\"type\":{\"kind\":\"String\"}}";
            var res = CadenceTypeInterpreter.ObjectFromCadenceJson(json);

            if (res is not IDictionary<string, object> reference)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.AreEqual("Reference", reference["kind"]);

            if (reference["authorization"] is not IDictionary<string, object> authorization)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.AreEqual("Unauthorized", authorization["kind"]);
            Assert.IsNull(authorization["entitlements"]);
        }

        [TestMethod]
        public void Reference_Cadence1_0()
        {
            var json = "{\"kind\":\"Reference\",\"authorization\":{\"kind\":\"EntitlementMapAuthorization\",\"entitlements\":[{\"kind\":\"EntitlementMap\",\"typeID\":\"foo\"}]},\"type\":{\"kind\":\"String\"}}";
            var res = CadenceTypeInterpreter.ObjectFromCadenceJson(json);

            if (res is not IDictionary<string, object> reference)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.AreEqual("Reference", reference["kind"]);

            if (reference["authorization"] is not IDictionary<string, object> authorization)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.AreEqual("EntitlementMapAuthorization", authorization["kind"]);

            if (authorization["entitlements"] is not IList<object> entitlements)
            {
                Assert.Fail("expected list");
                return;
            }

            Assert.AreEqual(1, entitlements.Count);

            if (entitlements.First() is not IDictionary<string, object> entitlement)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.AreEqual("EntitlementMap", entitlement["kind"]);
            Assert.AreEqual("foo", entitlement["typeID"]);

            if (reference["type"] is not IDictionary<string, object> type)
            {
                Assert.Fail("expected dictionary");
                return;
            }

            Assert.AreEqual("String", type["kind"]);
        }

        [TestMethod]
        public void VariableSizedArray_ContainsNullEntitlement_AndIntersection()
        {
            var json = @"{""kind"":""VariableSizedArray"",""type"":{""kind"":""Struct"",""type"":"""",""typeID"":""A.631e88ae7f1d7c20.MetadataViews.Royalty"",""initializers"":[],""fields"":[{""id"":""receiver"",""type"":{""kind"":""Capability"",""type"":{""kind"":""Reference"",""authorization"":{""kind"":""Unauthorized"",""entitlements"":null},""type"":{""kind"":""Intersection"",""typeID"":""{A.9a0766d93b6608b7.FungibleToken.Receiver}"",""types"":[{""kind"":""ResourceInterface"",""type"":"""",""typeID"":""A.9a0766d93b6608b7.FungibleToken.Receiver"",""initializers"":[],""fields"":[{""id"":""uuid"",""type"":{""kind"":""UInt64""}}]}]}}}},{""id"":""cut"",""type"":{""kind"":""UFix64""}},{""id"":""description"",""type"":{""kind"":""String""}}]}}";
            var res = CadenceTypeInterpreter.ObjectFromCadenceJson(json);

            var dict = res as IDictionary<string, object>;
            Assert.IsNotNull(dict);

            Assert.AreEqual("VariableSizedArray", dict["kind"]);

            var type = dict["type"] as IDictionary<string, object>;
            Assert.IsNotNull(type);

            Assert.AreEqual("Struct", type["kind"]);

            var fields = type["fields"] as IList<object>;
            Assert.IsNotNull(fields);

            var capabilityField = fields[0] as IDictionary<string, object>;
            Assert.IsNotNull(capabilityField);
            Assert.AreEqual("receiver", capabilityField["id"]);

            var capabilityType = capabilityField["type"] as IDictionary<string, object>;
            Assert.IsNotNull(capabilityType);
            Assert.AreEqual("Capability", capabilityType["kind"]);

            var innerType = capabilityType["type"] as IDictionary<string, object>;
            Assert.IsNotNull(innerType);
            Assert.AreEqual("Reference", innerType["kind"]);

            var auth = innerType["authorization"] as IDictionary<string, object>;
            Assert.IsNotNull(auth);
            Assert.AreEqual("Unauthorized", auth["kind"]);
            Assert.IsNull(auth["entitlements"]);
        }

        [TestMethod]
        public void OptionalType()
        {
            var json = @"{""kind"":""Optional"",""type"":{""kind"":""String""}}";
            var res = CadenceTypeInterpreter.ObjectFromCadenceJson(json);

            var dict = res as IDictionary<string, object>;
            Assert.IsNotNull(dict);
            Assert.AreEqual("Optional", dict["kind"]);

            var type = dict["type"] as IDictionary<string, object>;
            Assert.IsNotNull(type);
            Assert.AreEqual("String", type["kind"]);
        }
    }
}