using Graffle.FlowSdk.Services.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graffle.FlowSdk.Services.Tests.CadenceJsonTests;

[TestClass]
public class VoidTests
{
    [TestMethod]
    public void Void()
    {
        var json = "{\"type\":\"Void\"}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);
        Assert.AreEqual("Void", res);
    }
}
