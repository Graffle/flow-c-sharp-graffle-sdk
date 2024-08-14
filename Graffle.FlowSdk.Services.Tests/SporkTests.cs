using Graffle.FlowSdk.Services.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graffle.FlowSdk.Services.Tests
{
    [TestClass]
    public class SporkTests
    {
        [TestMethod]
        public void IsCrescendo_TestNet_ReturnsFalse()
        {
            var spork = Sporks.GetSporkByName("TestNet49");

            Assert.IsFalse(Sporks.IsCrescendo(spork));
        }

        [TestMethod]
        public void IsCrescendo_TestNet_ReturnsTrue()
        {
            var spork = Sporks.GetSporkByName("TestNet");

            Assert.IsTrue(Sporks.IsCrescendo(spork));
        }

        [TestMethod]
        public void IsCrescendo_MainNet_ReturnsFalse()
        {
            var spork = Sporks.GetSporkByName("MainNet");

            Assert.IsFalse(Sporks.IsCrescendo(spork));
        }
    }
}