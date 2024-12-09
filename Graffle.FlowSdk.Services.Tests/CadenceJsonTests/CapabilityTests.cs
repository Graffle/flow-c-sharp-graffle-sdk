using Graffle.FlowSdk.Services.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Graffle.FlowSdk.Services.Tests.CadenceJsonTests.ValueTests;

[TestClass]
public class CapabilityType
{
    [TestMethod]
    public void CapabilityType_LegacyJson()
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
    public void CapabilityType_SecureCadence()
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
    public void CapabilityType_Cadence1_0()
    {
        var json = "{\"type\":\"Capability\",\"value\":{\"id\":\"1\",\"address\":\"0x1\",\"borrowType\":{\"kind\":\"Int\"}}}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);
        if (res is not IDictionary<string, object> dict)
        {
            Assert.Fail("Expected dictionary");
            return;
        }

        Assert.AreEqual("1", dict["id"]);
        Assert.AreEqual("0x1", dict["address"]);

        if (dict["borrowType"] is not IDictionary<string, object> borrowType)
        {
            Assert.Fail("expected dictionary");
            return;
        }

        Assert.IsTrue(borrowType.ContainsKey("kind"));
        Assert.AreEqual("Int", borrowType["kind"]);
    }
}
