using Graffle.FlowSdk.Services.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Graffle.FlowSdk.Services.Tests.CadenceJsonTests.ValueTests;

[TestClass]
public class CompositeTests
{
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
}
