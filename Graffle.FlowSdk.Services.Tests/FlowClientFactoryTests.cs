using System;
using System.Threading.Tasks;
using Graffle.FlowSdk.Services.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graffle.FlowSdk.Services.Tests
{
    [TestClass]
    public class FlowClientFactoryTests
    {
        [TestMethod]
        public void CreateSingleClient()
        {
            using var flowClientFactory = new FlowClientFactory(NodeType.MainNet);
            var spork = Sporks.GetSporkByName(Nodes.MainNetSporks.MainNet.Name);
            var flowClient = flowClientFactory.CreateFlowClient(spork);
            Assert.AreEqual(MainNetSporks.MainNet.Name, flowClient.CurrentSpork.Name);
        }

        [TestMethod]
        public void CreateMultipleClient()
        {
            using var flowClientFactory = new FlowClientFactory(NodeType.MainNet);
            var mainSpork = Sporks.GetSporkByName(Nodes.MainNetSporks.MainNet.Name);
            var flowClient = flowClientFactory.CreateFlowClient(mainSpork);

            var mainSpork1 = Sporks.GetSporkByName(Nodes.MainNetSporks.MainNet1.Name);
            var flowClient2 = flowClientFactory.CreateFlowClient(mainSpork1);
            var flowClient3 = flowClientFactory.CreateFlowClient(mainSpork1);

            Assert.AreEqual(MainNetSporks.MainNet.Name, flowClient.CurrentSpork.Name);
            Assert.AreEqual(MainNetSporks.MainNet1.Name, flowClient2.CurrentSpork.Name);
            Assert.AreEqual(flowClient3, flowClient2);
        }

        [TestMethod]
        public void MultipleClients_CacheOverride()
        {
            using var flowClientFactory = new FlowClientFactory(NodeType.MainNet);
            var mainSpork = Sporks.GetSporkByName(Nodes.MainNetSporks.MainNet.Name);
            var flowClient = flowClientFactory.CreateFlowClient(mainSpork);

            var mainSpork1 = Sporks.GetSporkByName(Nodes.MainNetSporks.MainNet1.Name);
            var flowClient2 = flowClientFactory.CreateFlowClient(mainSpork1);
            var flowClient3 = flowClientFactory.CreateFlowClient(mainSpork1, true);

            Assert.AreEqual(MainNetSporks.MainNet.Name, flowClient.CurrentSpork.Name);
            Assert.AreEqual(MainNetSporks.MainNet1.Name, flowClient2.CurrentSpork.Name);

            //these should be different references 
            Assert.AreNotEqual(flowClient3, flowClient2);
        }

        [TestMethod]
        public void CustomAccessNode()
        {
            using var fcf = new FlowClientFactory(NodeType.MainNet);

            const string uri1 = "google.com";
            const string uri2 = "bing.com";

            var fc1 = fcf.CreateFlowClientFromUri(uri1);
            var fc2 = fcf.CreateFlowClientFromUri(uri2);

            Assert.AreNotSame(fc1, fc2);
        }

        [TestMethod]
        [DataRow("TestNet")]
        [DataRow("MainNet")]
        public void CreateFlowClient_ChoosesCorrectSpork(string sporkName)
        {
            var spork = Sporks.GetSporkByName(sporkName);

            var fcf = new FlowClientFactory(sporkName);
            var fc = fcf.CreateFlowClient();

            Assert.AreEqual(spork.Name, fc.CurrentSpork.Name);
            Assert.AreEqual(spork.Node, fc.CurrentSpork.Node);
        }
    }
}