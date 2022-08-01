using Graffle.FlowSdk.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Graffle.FlowSdk.Services.Tests.SerializationTests
{
    [TestClass]
    public class ScriptResultTests
    {
        [TestMethod]
        public async Task Dictionary_Struct_Type_DeserializesCorrectlyToPrimitive()
        {
            const string script = @"import NFTCatalog from 0x49a7cda3a1eecc29
                                    pub fun main() : { String: NFTCatalog.NFTCatalogMetadata} {
                                        return NFTCatalog.getCatalog()
                                    }";

            using var flowClientFactory = new FlowClientFactory("MainNet");
            var flowClient = flowClientFactory.CreateFlowClient();
            var latestBlock = await flowClient.GetLatestBlockAsync();
            var scriptBytes = Encoding.UTF8.GetBytes(script);

            var scriptResponse = await flowClient.ExecuteScriptAtBlockHeightAsync(latestBlock.Height, scriptBytes, new List<FlowValueType>());
            var scriptResponseJson = Encoding.UTF8.GetString(scriptResponse.Value.ToByteArray());

            var flowType = DictionaryType.FromJson(scriptResponseJson);
            var metaData = flowType.ConvertToObject();

            //verify data deserialized correctly
            //get one item from the dictionary

            var momentables = metaData["momentables"] as Dictionary<string, object>;
            Assert.IsNotNull(momentables);

            var nftType = momentables["nftType"] as Dictionary<string, object>;
            Assert.IsNotNull(nftType);

            //verify data from the type
            var kind = nftType["kind"];
            Assert.AreEqual("Resource", kind);

            var type = nftType["type"];
            Assert.AreEqual(string.Empty, type);

            var typeId = nftType["typeID"];
            Assert.AreEqual("A.9d21537544d9123d.Momentables.NFT", typeId);

            var initializers = nftType["initializers"] as List<object>;
            Assert.AreEqual(0, initializers.Count);

            var fields = nftType["fields"] as List<object>;
            Assert.AreEqual(11, fields.Count);
        }
    }
}