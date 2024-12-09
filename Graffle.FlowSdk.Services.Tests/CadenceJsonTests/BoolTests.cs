using Graffle.FlowSdk.Services.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graffle.FlowSdk.Services.Tests.CadenceJsonTests.ValueTests;

[TestClass]
public class BoolTests
{
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
}
