using Graffle.FlowSdk.Services.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graffle.FlowSdk.Services.Tests.CadenceJsonTests.ValueTests;

[TestClass]
public class AddressTests
{
    [TestMethod]
    public void AddressType()
    {
        var json = @"{""type"":""Address"",""value"":""0x66d6f450e25a4e22""}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        Assert.IsInstanceOfType<string>(res);
        Assert.AreEqual("0x66d6f450e25a4e22", res);
    }
}
