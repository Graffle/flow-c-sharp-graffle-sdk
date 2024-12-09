using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Graffle.FlowSdk.Services.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graffle.FlowSdk.Services.Tests.CadenceJsonTests.ValueTests;

[TestClass]
public class RangeTests
{
    [TestMethod]
    public void InclusiveRange()
    {
        var json = "{\"type\":\"InclusiveRange\",\"value\":{\"start\":{\"type\":\"Int256\",\"value\":\"10\"},\"end\":{\"type\":\"Int8\",\"value\":\"20\"},\"step\":{\"type\":\"Int128\",\"value\":\"5\"}}}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        if (res is not IDictionary<string, object> dict)
        {
            Assert.Fail("expected dictionary");
            return;
        }

        if (dict["start"] is not IDictionary<string, object> start)
        {
            Assert.Fail("expected dictionary");
            return;
        }

        Assert.AreEqual("Int256", start["type"]);
        Assert.AreEqual("10", start["value"]);

        if (dict["end"] is not IDictionary<string, object> end)
        {
            Assert.Fail("expected dictionary");
            return;
        }

        Assert.AreEqual("Int8", end["type"]);
        Assert.AreEqual("20", end["value"]);

        if (dict["step"] is not IDictionary<string, object> step)
        {
            Assert.Fail("expected dictionary");
            return;
        }

        Assert.AreEqual("Int128", step["type"]);
        Assert.AreEqual("5", step["value"]);
    }
}
