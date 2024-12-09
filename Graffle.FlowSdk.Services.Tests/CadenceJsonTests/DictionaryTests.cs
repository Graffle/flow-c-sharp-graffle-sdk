using Graffle.FlowSdk.Services.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Graffle.FlowSdk.Services.Tests.CadenceJsonTests.ValueTests;

[TestClass]
public class DictionaryTests
{
    [TestMethod]
    public void DictionaryType()
    {
        var json = @"{""type"":""Dictionary"",""value"":[{""key"":{""type"":""UInt8"",""value"":""123""},""value"":{""type"":""String"",""value"":""test""}}]}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        Assert.IsNotNull(res);
        if (res is not IDictionary<string, object> dict)
        {
            Assert.Fail("expected dictionary");
            return;
        }

        Assert.AreEqual(1, dict.Keys.Count);
        Assert.IsTrue(dict.ContainsKey("123"));
        Assert.AreEqual("test", dict["123"]); ;
    }
}
