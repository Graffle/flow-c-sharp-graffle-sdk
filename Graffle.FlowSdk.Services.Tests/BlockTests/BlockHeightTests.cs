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
    public class BlockHeightTests
    {
        private IGraffleClient flowClient { get; }

        public BlockHeightTests()
        {
            var flowClientFactory = new FlowClientFactory(NodeType.Emulator);
            var spork = Sporks.GetSporkByName(Nodes.EmulatorSporks.Emulator.Name);
            this.flowClient = flowClientFactory.CreateFlowClient(spork);
        }

        [TestMethod]
        [Ignore]
        public async Task When_GetLatestBlockAsync_Then_Return_Latest_Block()
        {
            var latestBlockResponse = await this.flowClient.GetLatestBlockAsync(true);
            Assert.IsNotNull(latestBlockResponse);
            Assert.IsNotNull(latestBlockResponse.Id);
            Assert.AreNotEqual(latestBlockResponse.Id.HashToByteString().ToBase64(), "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=");
            Assert.IsNotNull(latestBlockResponse.Timestamp);
            Assert.IsNotNull(latestBlockResponse.CollectionGuarantees);
            Assert.IsNotNull(latestBlockResponse.Signatures);
        }

        [TestMethod]
        [Ignore]
        public async Task When_GetBlockByHeightAsync_Then_Return_Block()
        {
            var latestBlockResponse = await this.flowClient.GetBlockByHeightAsync(0);
            Assert.IsNotNull(latestBlockResponse);
            Assert.IsNotNull(latestBlockResponse.Id);
            Assert.AreNotEqual(latestBlockResponse.Id.HashToByteString().ToBase64(), "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=");
            Assert.IsNotNull(latestBlockResponse.Timestamp);
            Assert.IsNotNull(latestBlockResponse.CollectionGuarantees);
            Assert.IsNotNull(latestBlockResponse.Signatures);
        }

        [TestMethod]
        [Ignore]
        public async Task Given_HelloWorld_When_ExecuteScriptAtBlockHeightAsync_Then_Return_Successful_Result()
        {
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

        [TestMethod]
        [Ignore]
        public async Task Ryan_Test()
        {
            var s = @"
                        import NonFungibleToken from 0x1d7e57aa55817448
                        import MetadataViews from 0x1d7e57aa55817448
                        import FungibleToken from 0xf233dcee88fe0abe
                        import FlowToken from 0x1654653399040a61
                        import GoatedGoats from 0x2068315349bdfce5

                        pub fun main(id: UInt64, address: Address): {String: AnyStruct} {
                            let account = getAccount(address)
                            let goatPubPath = GoatedGoats.CollectionPublicPath
                            let goatCollection = account.getCapability<&GoatedGoats.Collection{NonFungibleToken.CollectionPublic,NonFungibleToken.Receiver,GoatedGoats.GoatCollectionPublic,MetadataViews.ResolverCollection}>(goatPubPath).borrow()!
                            let goat = goatCollection.borrowGoat(id: id)!
                            return {
                                ""traitActions"": goat.traitActions,
                                ""lastTraitActionDate"": goat.lastTraitActionDate,
                                ""goatCreationDate"": goat.goatCreationDate,
                                ""traitSlots"": GoatedGoats.getEditionTraitSlots(goat.goatID),
                                ""equippedTraits"": goat.getEquippedTraits(),
                                ""compositeSkinBodyCID"": GoatedGoats.getEditionMetadata(goat.goatID)[""compositeSkinBodyCID""] ?? """",
                                ""compositeSkinHeadCID"": GoatedGoats.getEditionMetadata(goat.goatID)[""compositeSkinHeadCID""] ?? """"
                            }
                            }
                        ";

            var factory = new FlowClientFactory(NodeType.MainNet);
            var fc = factory.CreateFlowClient();

            var arg1 = new UInt64Type(8243);
            var arg2 = new AddressType("0x8a02f31896be25f9");

            var scriptBytes = Encoding.ASCII.GetBytes(s);
            var latestBlock = await fc.GetLatestBlockAsync();
            var scriptResponse = await fc.ExecuteScriptAtBlockHeightAsync(latestBlock.Height, scriptBytes, new List<FlowValueType>() { arg1, arg2 });
            var metaDataJson = Encoding.Default.GetString(scriptResponse.Value.ToByteArray());
            var result = DictionaryType.FromJson(metaDataJson);

            JsonSerializerOptions options = new();
            options.Converters.Add(new FlowCompositeTypeConverter());
            options.Converters.Add(new GraffleCompositeTypeConverter());
            options.Converters.Add(new FlowValueTypeConverter());

            var resTest = System.Text.Json.JsonSerializer.Deserialize<FlowValueType>(metaDataJson, options);
            Assert.IsInstanceOfType(resTest, typeof(DictionaryType));
        }
    }
}