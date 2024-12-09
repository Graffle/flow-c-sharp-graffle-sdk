using Graffle.FlowSdk.Services.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graffle.FlowSdk.Services.Tests.CadenceJsonTests.ValueTests;

[TestClass]
public class StringTests
{
    [TestMethod]
    public void StringType_WithLineFeed()
    {
        var json = "{\"type\":\"String\",\"value\":\"Appearance: The xG Reward for players with game time in a fixture.\n\nGet xG Rewards for your football achievements.\nBuild your collection - your story.\nUnlock xG experiences.\n\nhttps://linktr.ee/xgstudios\"}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        Assert.AreEqual("Appearance: The xG Reward for players with game time in a fixture.\n\nGet xG Rewards for your football achievements.\nBuild your collection - your story.\nUnlock xG experiences.\n\nhttps://linktr.ee/xgstudios", res);
    }

    [TestMethod]
    public void StringType_Simple()
    {
        var json = "{\"type\":\"String\",\"value\":\"foo\"}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        Assert.AreEqual("foo", res);
    }
}
