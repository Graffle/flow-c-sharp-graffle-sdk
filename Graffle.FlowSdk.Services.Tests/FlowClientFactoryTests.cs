using System;
using System.Threading.Tasks;
using Graffle.FlowSdk.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graffle.FlowSdk.Services.Tests
{

    [TestClass]
    public class FlowClientFactoryTests
    {
        [TestMethod]
        public void CreateSingleClient()
        {
            var flowClientFactory = new FlowClientFactory(NodeType.MainNet);
            var spork = Sporks.GetSporkByName(Nodes.MainNetSporks.MainNet.Name);
            var flowClient = flowClientFactory.CreateFlowClient(spork);
            Assert.AreEqual(MainNetSporks.MainNet.Name, flowClient.CurrentSpork.Name);
        }


         [TestMethod]
        public void CreateMultipleClient()
        {
            var flowClientFactory = new FlowClientFactory(NodeType.MainNet);
            var mainSpork = Sporks.GetSporkByName(Nodes.MainNetSporks.MainNet.Name);
            var flowClient = flowClientFactory.CreateFlowClient(mainSpork);

            var mainSpork1 = Sporks.GetSporkByName(Nodes.MainNetSporks.MainNet1.Name);
            var flowClient2 = flowClientFactory.CreateFlowClient(mainSpork1);
            var flowClient3 = flowClientFactory.CreateFlowClient(mainSpork1);
            
            Assert.AreEqual(MainNetSporks.MainNet.Name, flowClient.CurrentSpork.Name);
            Assert.AreEqual(MainNetSporks.MainNet1.Name, flowClient2.CurrentSpork.Name);
            Assert.AreEqual(flowClient3, flowClient2);
        }
    }

}