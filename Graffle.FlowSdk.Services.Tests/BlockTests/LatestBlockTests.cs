using System.Threading.Tasks;
using Graffle.FlowSdk.Services.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graffle.FlowSdk.Services.Tests.BlockTests
{
    [TestClass]
    public class LatestBlockTests
    {
        [TestMethod]
        public async Task GetLatestBlockAsync_MainNet_Succeeds()
        {
            var factory = new FlowClientFactory(Nodes.NodeType.MainNet);
            var fc = factory.CreateFlowClient(MainNetSporks.MainNet.Name);

            var block = await fc.GetLatestBlockAsync(true);

            Assert.IsNotNull(block);
            Assert.IsTrue(block.Height > MainNetSporks.MainNet.ROOT_HEIGHT);
        }

        [TestMethod]
        [Ignore]
        public async Task GetLatestBlockAsync_TestNet_Succeeds()
        {
            var factory = new FlowClientFactory(Nodes.NodeType.TestNet);
            var fc = factory.CreateFlowClient(TestNetSporks.TestNet.Name);

            var block = await fc.GetLatestBlockAsync(true);

            Assert.IsNotNull(block);
            Assert.IsTrue(block.Height > TestNetSporks.TestNet.ROOT_HEIGHT);
        }
    }
}