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
        public static IEnumerable<object[]> SPORKS() =>
         new[]
         {
            new [] { Sporks.GetSporkByName("MainNet21") },
            new [] { Sporks.GetSporkByName("MainNet") },
            new [] { Sporks.GetSporkByName("TestNet") },
            //new [] { Sporks.GetSporkByName("TestNet40") }
         };

        [TestMethod]
        [DynamicData(nameof(SPORKS), DynamicDataSourceType.Method)]
        public async Task PingAsync(Spork spork)
        {
            var client = new GraffleClient(spork);
            Assert.IsTrue(await client.PingAsync());
        }

        [TestMethod]
        [DynamicData(nameof(SPORKS), DynamicDataSourceType.Method)]
        public async Task GetLatestBlockAsync(Spork spork)
        {
            var client = new GraffleClient(spork);
            var res = await client.GetLatestBlockAsync(true);

            Assert.IsNotNull(res);
            Assert.IsTrue(res.Height > 0);
        }

        [TestMethod]
        [DynamicData(nameof(SPORKS), DynamicDataSourceType.Method)]
        public async Task ExecuteScriptAtBlockHeightAsync(Spork spork)
        {
            const string script = @"pub fun main(arg : String): String { return arg }";
            var bytes = Encoding.UTF8.GetBytes(script);

            var client = new GraffleClient(spork);
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