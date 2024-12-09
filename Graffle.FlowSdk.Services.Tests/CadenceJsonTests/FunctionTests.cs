using Graffle.FlowSdk.Services.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Graffle.FlowSdk.Services.Tests.CadenceJsonTests;

[TestClass]
public class FunctionTests
{
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
}
