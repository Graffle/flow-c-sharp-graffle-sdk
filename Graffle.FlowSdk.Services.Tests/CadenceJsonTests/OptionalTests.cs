using Graffle.FlowSdk.Services.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graffle.FlowSdk.Services.Tests.CadenceJsonTests.ValueTests;

[TestClass]
public class OptionalTests
{
    [TestMethod]
    public void OptionalType_Null()
    {
        var json = "{\"type\":\"Optional\",\"value\":null}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);
        Assert.IsNull(res);
    }

    [TestMethod]
    public void Optional_StringType()
    {
        var json = "{\"type\":\"Optional\",\"value\":{\"type\":\"String\",\"value\":\"Derrick \\\"The Black Beast\\\" Lewis delivers a lights out uppercut against Curtis Blaydes, to break the record for the most KOs in UFC heavyweight history and tie for the most KO/TKO wins in UFC history\"}}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        Assert.AreEqual("Derrick \"The Black Beast\" Lewis delivers a lights out uppercut against Curtis Blaydes, to break the record for the most KOs in UFC heavyweight history and tie for the most KO/TKO wins in UFC history", res);
    }
}
