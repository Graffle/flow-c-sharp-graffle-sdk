using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Graffle.FlowSdk.Services.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graffle.FlowSdk.Services.Tests
{
    [TestClass]
    public class GraffleClientFactoryTests
    {
        [TestMethod]
        public async Task Create_CurrentSpork()
        {
            var factory = new GraffleClientFactory("MainNet");
            var client = factory.Create();

            Assert.IsNotNull(client);
            Assert.IsTrue(await client.PingAsync());

            //get another client
            var client2 = factory.Create();
            Assert.IsNotNull(client2);
            Assert.IsTrue(await client2.PingAsync());
        }

        [TestMethod]
        public async Task Create_OldSpork()
        {
            var factory = new GraffleClientFactory("MainNet");
            var client = factory.Create(MainNetSporks.MainNet20.ROOT_HEIGHT + 5000ul);

            Assert.IsNotNull(client);
            Assert.IsTrue(await client.PingAsync());

            //get another client at a differeent sork
            var client2 = factory.Create(MainNetSporks.MainNet15.ROOT_HEIGHT + 5000ul);
            Assert.IsNotNull(client2);
        }

        [TestMethod]
        public async Task Create_CustomAccessNodeUri()
        {
            var factory = new GraffleClientFactory("MainNet");
            var client = factory.Create("access.mainnet.nodes.onflow.org:9000");

            Assert.IsNotNull(client);
            Assert.IsTrue(await client.PingAsync());
        }
    }
}