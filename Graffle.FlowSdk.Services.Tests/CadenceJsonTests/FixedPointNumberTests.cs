using Graffle.FlowSdk.Services.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graffle.FlowSdk.Services.Tests.CadenceJsonTests.ValueTests;

[TestClass]
public class FixedPointNumberTests
{
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
}
