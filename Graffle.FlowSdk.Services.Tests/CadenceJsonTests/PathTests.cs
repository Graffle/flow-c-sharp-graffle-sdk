using Graffle.FlowSdk.Services.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Graffle.FlowSdk.Services.Tests.CadenceJsonTests;

[TestClass]
public class PathTests
{
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
}
