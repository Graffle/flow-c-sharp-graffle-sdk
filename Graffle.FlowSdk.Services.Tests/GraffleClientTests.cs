using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graffle.FlowSdk.Services.Nodes;
using Graffle.FlowSdk.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graffle.FlowSdk.Services.Tests
{
    [TestClass]
    public class GraffleClientTests
    {
        private static Spork MAIN_NET = Sporks.GetSporkByName("MainNet");

        [TestMethod]
        public async Task PingAsync()
        {
            var client = new GraffleClient(MAIN_NET);
            Assert.IsTrue(await client.PingAsync());
        }

        [TestMethod]
        public async Task GetLatestBlockAsync()
        {
            var client = new GraffleClient(MAIN_NET);
            var res = await client.GetLatestBlockAsync(true);

            Assert.IsNotNull(res);
            Assert.IsTrue(res.Height > 0);
        }

        [TestMethod]
        public async Task ExecuteScriptAtBlockHeightAsync()
        {
            const string script = @"pub fun main(arg : String): String { return arg }";
            var bytes = Encoding.UTF8.GetBytes(script);

            var client = new GraffleClient(MAIN_NET);
            var arg = new StringType("foo");
            var latestBlock = await client.GetLatestBlockAsync(true);

            var res = await client.ExecuteScriptAtBlockHeightAsync(latestBlock.Height, bytes, new List<FlowValueType>() { arg });
            var json = Encoding.UTF8.GetString(res.Value.ToByteArray());
            json = json.Replace('\n', ' '); //why does flow put a newline here???????????

            var stringType = StringType.FromJson(json);
            Assert.AreEqual("foo", stringType.Data);
        }
    }
}