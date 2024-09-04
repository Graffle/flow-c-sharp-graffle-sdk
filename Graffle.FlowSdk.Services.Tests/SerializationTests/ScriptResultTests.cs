using Graffle.FlowSdk.Services.Serialization;
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
        [Ignore]
        public async Task Dictionary_Struct_Type_DeserializesCorrectlyToPrimitive()
        {
            const string script = @"import NFTCatalog from 0x49a7cda3a1eecc29
                                    access(all) fun main(batch : [UInt64]): {String : NFTCatalog.NFTCatalogMetadata} {
                                        if batch == nil {
                                            return NFTCatalog.getCatalog()
                                        }
                                        let catalog = NFTCatalog.getCatalog()
                                        let catalogIDs = catalog.keys
                                        var data : {String : NFTCatalog.NFTCatalogMetadata} = {}
                                        var i = batch![0]
                                        while i < batch![1] {
                                            data.insert(key: catalogIDs[i], catalog[catalogIDs[i]]!)
                                            i = i + 1
                                        }
                                        return data
                                    }";

            using var flowClientFactory = new FlowClientFactory("MainNet") { CandeceSerializer = CadenceSerializerVersion.Expando };
            var flowClient = flowClientFactory.CreateFlowClient();
            var latestBlock = await flowClient.GetLatestBlockAsync();
            var scriptBytes = Encoding.UTF8.GetBytes(script);

            var arg1 = new UInt64Type(0);
            var arg2 = new UInt64Type(50);
            var array = new ArrayType(new List<FlowValueType> { arg1, arg2 });

            var scriptResponse = await flowClient.ExecuteScriptAtBlockHeightAsync(latestBlock.Height, scriptBytes, new List<FlowValueType>() { array });
            var scriptResponseJson = Encoding.UTF8.GetString(scriptResponse.Value.ToByteArray());

            // var flowType = DictionaryType.FromJson(scriptResponseJson);
            // var metaData = flowType.ConvertToObject();
            var metaData = CadenceJsonInterpreter.ObjectFromCadenceJson(scriptResponseJson) as Dictionary<string, dynamic>;

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