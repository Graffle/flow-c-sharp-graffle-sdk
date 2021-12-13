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
    [Ignore]
    public class BlockIdTests
    {
        private GraffleClient flowClient { get; }
        
        public BlockIdTests(){
            var flowClientFactory = new FlowClientFactory(NodeType.Emulator);
            var spork = Sporks.GetSporkByName(Nodes.EmulatorSporks.Emulator.Name);
            this.flowClient = flowClientFactory.CreateFlowClient(spork);
        }        

        [TestMethod]
        public async Task Given_HelloWorld_When_ExecuteScriptAtBlockIdAsync_Then_Return_Successful_Result(){
            var latestBlockResponse = await this.flowClient.GetLatestBlockAsync(true);

            var helloWorldScript = @"
                pub fun main(): String {
                    return ""Hello World""
                }               
            ";
            
            var scriptBytes = Encoding.ASCII.GetBytes(helloWorldScript);
            
            var scriptResponse = await flowClient.ExecuteScriptAtBlockIdAsync(latestBlockResponse.Id, scriptBytes, new List<FlowValueType>());
            var metaDataJson = Encoding.Default.GetString(scriptResponse.Value.ToByteArray());
            var result = StringType.FromJson(metaDataJson);

            Assert.AreEqual(result.Data, "Hello World");
            Assert.AreEqual(result.Type, "String");
        }
    }
}