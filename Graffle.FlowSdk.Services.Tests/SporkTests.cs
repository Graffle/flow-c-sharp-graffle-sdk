using Graffle.FlowSdk.Services.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graffle.FlowSdk.Services.Tests
{
    [TestClass]
    public class SporkTests
    {
        [TestMethod]
        public void IsCrescendo_OldTestNetSporks_ReturnsFalse()
        {
            for (int i = 17; i <= 50; i++)
            {
                var sporkName = $"TestNet{i}";
                var spork = Sporks.GetSporkByName(sporkName);

                Assert.IsFalse(Sporks.IsCrescendo(spork));
            }
        }

        [TestMethod]
        public void IsCrescendo_TestNet_ReturnsTrue()
        {
            var spork = Sporks.GetSporkByName("TestNet");

            Assert.IsTrue(Sporks.IsCrescendo(spork));
        }

        [TestMethod]
        public void IsCrescendo_TestNet51_ReturnsTrue()
        {
            var spork = Sporks.GetSporkByName("TestNet51");

            Assert.IsTrue(Sporks.IsCrescendo(spork));
        }

        [TestMethod]
        public void IsCrescendo_MainNet_ReturnsTrue()
        {
            var spork = Sporks.GetSporkByName("MainNet");

            Assert.IsTrue(Sporks.IsCrescendo(spork));
        }

        [TestMethod]
        public void IsCrescendo_MainNet25_ReturnsTrue()
        {
            var spork = Sporks.GetSporkByName("MainNet25");

            Assert.IsTrue(Sporks.IsCrescendo(spork));
        }

        [TestMethod]
        public void IsCrescendo_OldMainNetSporks_ReturnsFalse()
        {
            for (int i = 1; i <= 24; i++)
            {
                var sporkName = $"MainNet{i}";
                var spork = Sporks.GetSporkByName(sporkName);

                Assert.IsFalse(Sporks.IsCrescendo(spork));
            }
        }
    }
}