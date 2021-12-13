using System.Collections.Generic;
using System.Threading.Tasks;
using Graffle.FlowSdk.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Text.Json;
using Graffle.FlowSdk.Services.Nodes;

namespace Graffle.FlowSdk.Services.Tests.BlockTests
{
    [TestClass]
    //[Ignore]
    public class BlockHeightTests
    {
        private IGraffleClient flowClient { get; }

        public BlockHeightTests(){
            var flowClientFactory = new FlowClientFactory(NodeType.Emulator);
            var spork = Sporks.GetSporkByName(Nodes.EmulatorSporks.Emulator.Name);
            this.flowClient = flowClientFactory.CreateFlowClient(spork);
        }

        [TestMethod]
        public async Task When_GetLatestBlockAsync_Then_Return_Latest_Block(){
            var latestBlockResponse = await this.flowClient.GetLatestBlockAsync(true);
            Assert.IsNotNull(latestBlockResponse);
            Assert.IsNotNull(latestBlockResponse.Id);
            Assert.AreNotEqual(latestBlockResponse.Id.ToBase64(), "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=");
            Assert.IsNotNull(latestBlockResponse.Timestamp);
            Assert.IsNotNull(latestBlockResponse.RawBlock.CollectionGuarantees);
            Assert.IsNotNull(latestBlockResponse.RawBlock.Signatures);
        }

        [TestMethod]
        public async Task When_GetBlockByHeightAsync_Then_Return_Block(){
            var latestBlockResponse = await this.flowClient.GetBlockByHeightAsync(0);
            Assert.IsNotNull(latestBlockResponse);
            Assert.IsNotNull(latestBlockResponse.Id);
            Assert.AreNotEqual(latestBlockResponse.Id.ToBase64(), "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=");
            Assert.IsNotNull(latestBlockResponse.Timestamp);
            Assert.IsNotNull(latestBlockResponse.RawBlock.CollectionGuarantees);
            Assert.IsNotNull(latestBlockResponse.RawBlock.Signatures);
        }

        [TestMethod]
        public async Task Given_HelloWorld_When_ExecuteScriptAtBlockHeightAsync_Then_Return_Successful_Result(){
            var helloWorldScript = @"
                pub fun main(): String {
                    return ""Hello World""
                }
            ";

            var scriptBytes = Encoding.ASCII.GetBytes(helloWorldScript);

            var scriptResponse = await flowClient.ExecuteScriptAtBlockHeightAsync(0, scriptBytes, new List<FlowValueType>());
            var metaDataJson = Encoding.Default.GetString(scriptResponse.Value.ToByteArray());
            var result = StringType.FromJson(metaDataJson);

            Assert.AreEqual(result.Data, "Hello World");
            Assert.AreEqual(result.Type, "String");
        }
    }
}