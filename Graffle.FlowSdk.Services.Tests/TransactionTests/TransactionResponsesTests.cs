using Graffle.FlowSdk.Services.Models;
using Graffle.FlowSdk.Services.Nodes;
using Graffle.FlowSdk.Services.Serialization;
using Graffle.FlowSdk.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Graffle.FlowSdk.Services.Tests.TransactionsTests
{
    [TestClass]
    public class TransactionResponsesTests
    {
        private static FlowClientFactory _main;
        private static FlowClientFactory _test;

        [ClassInitialize]
        public static void ClassInit(TestContext ctx)
        {
            _main = new FlowClientFactory("MainNet");
            _test = new FlowClientFactory("TestNet");
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _main?.Dispose();
            _test?.Dispose();
        }

        [TestMethod]
        public void Transaction_Dictionary_With_UInt64Key()
        {
            var json = @"{
                        ""type"": ""Dictionary"",
                        ""value"": [
                            {
                            ""key"": {
                                ""type"": ""UInt64"",
                                ""value"": ""123456""
                            },
                            ""value"": {
                                ""type"": ""String"",
                                ""value"": ""test""
                            }
                            }
                        ]
                        }";

            var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);
            Assert.AreEqual("test", res["123456"]);
        }

        [TestMethod]
        public async Task Transaction_StructContainsOptionalStruct()
        {
            var res = await GetTransaction(80312030, "a586032cc7b3ccab93a68125b4252bbcd31dca8d1545432e6a05b7de5f12c880", NodeType.MainNet);
            var events = res.Events;
            var ev = events.FirstOrDefault(ev => ev.Type == "A.9a57dfe5c8ce609c.SoulMadeMarketplace.SoulMadeForSale");

            //let's validate the grafflecomposite
            var graffleComposite = ev.EventComposite;
            var data = graffleComposite.Data;
            Assert.AreEqual(CadenceSerializerVersion.Crescendo, graffleComposite.SerializerVersion);
            Assert.IsNotNull(data);
            Assert.AreEqual(4, data.Count);

            //pull out the field with a nested struct
            var nestedFields = data["saleData"]["mainDetail"]["componentDetails"];
            Assert.IsNotNull(nestedFields);
            Assert.IsInstanceOfType(nestedFields, typeof(List<object>));
            Assert.AreEqual(6, nestedFields.Count); //10 fields in the nested struct
        }

        [TestMethod] //backwards compatibility - this txn has the old json structure for Type
        public async Task Serialize_ArrayContainsStructs_OnlyStructFieldsAreSerialized()
        {
            var res = await GetTransaction(24684541, "c6043a12ddab4740d6dbb27a9171062813b0fff05f0a03529c61c620311be8e4", NodeType.MainNet);
            var events = res.Events;
            var ev = events.FirstOrDefault(ev => ev.Type == "A.8b148183c28ff88f.GaiaOrder.OrderAvailable");
            var composite = ev.EventComposite;

            Assert.IsNotNull(composite);
            var compositeData = composite.Data;
            Assert.IsNotNull(composite.Data);

            //verify that *only* the struct's fields were serialized
            var arrayData = compositeData["payments"];
            Assert.IsNotNull(arrayData);
            Assert.IsInstanceOfType(arrayData, typeof(List<object>));

            //check items in the array
            //these should be dictionaries (ie struct fields) rather than GraffleCompositeType
            foreach (var obj in arrayData)
            {
                Assert.IsInstanceOfType(obj, typeof(Dictionary<string, object>));
            }

            //let's take a look at one
            var structFields = arrayData[0] as Dictionary<string, object>;
            Assert.AreEqual(4, structFields.Count);
        }

        [TestMethod]
        public async Task Serialize_LargeTransaction_Succeeds()
        {
            var res = await GetTransaction(25568454, "64a8781b0c6873d98ec30d5ef6ee296dcdddf93a8c2ec2e4378a6cfaea6b2631", NodeType.MainNet);
            var events = res.Events;

            Assert.AreEqual(29625, events.Count());
        }

        [TestMethod]
        public async Task Serialize_SecureCadence_DictionaryType()
        {
            var res = await GetTransaction(31742217, "286ddea041ea32b0dfd34ff2e904e834f1e8bd299840919e6105497f5e409096", NodeType.MainNet);

            var nftListingAvailable = res.Events[0];

            var composite = nftListingAvailable.EventComposite;
            var data = composite.Data;

            //pull out the dictionary
            var nftType = data["nftType"];

            var nftTypeDict = nftType as Dictionary<string, object>;

            var fields = nftTypeDict["fields"];

            var fieldsList = fields as List<object>;

            var dictionaryType = fieldsList[4];

            var dictionaryDict = dictionaryType as Dictionary<string, object>;

            Assert.AreEqual("metadata", dictionaryDict["id"]);

            var typeDict = dictionaryDict["type"] as Dictionary<string, object>;

            var key = typeDict["key"];

            var keyDict = key as Dictionary<string, object>;
            Assert.AreEqual("String", keyDict["kind"]);
        }

        [TestMethod]
        public async Task SecureCadence_RepeatedTypeInRestrictedType()
        {
            //https://flowscan.org/transaction/f5c22ee398f9d5d8d2aace1cd689375fcebd300a6727aa963d58fc4fef204756
            var res = await GetTransaction(31739093, "f5c22ee398f9d5d8d2aace1cd689375fcebd300a6727aa963d58fc4fef204756", NodeType.MainNet);

            var ev = res.Events[0]; //A.4eb8a10cb9f87357.NFTStorefront.ListingAvailable

            var composite = ev.EventComposite.Data;

            //let's dig into the event composite and verify the repeated type was serialized properly into the graffle composite
            Assert.IsTrue(composite.ContainsKey("nftType"));
            var nftType = composite["nftType"];
            Assert.IsInstanceOfType(nftType, typeof(Dictionary<string, object>));

            var nftTypeDictionary = nftType as Dictionary<string, object>;
            Assert.IsTrue(nftTypeDictionary.ContainsKey("fields"));
            var nftTypeFields = nftType["fields"];
            Assert.IsInstanceOfType(nftTypeFields, typeof(List<object>));

            var nftTypeFieldsList = nftTypeFields as List<object>;
            var collaborators = nftTypeFieldsList[9];
            Assert.IsInstanceOfType(collaborators, typeof(Dictionary<string, object>));

            var collaboratorsDictionary = collaborators as Dictionary<string, object>;
            Assert.IsTrue(collaboratorsDictionary.ContainsKey("id"));
            Assert.AreEqual("collaborators", collaboratorsDictionary["id"]);
            Assert.IsTrue(collaboratorsDictionary.ContainsKey("type"));
            var collaboratorsType = collaboratorsDictionary["type"];
            Assert.IsInstanceOfType(collaboratorsType, typeof(Dictionary<string, object>));

            var collaboratorVariableArray = collaboratorsType as Dictionary<string, object>;
            Assert.IsTrue(collaboratorVariableArray.ContainsKey("kind"));
            Assert.AreEqual("VariableSizedArray", collaboratorVariableArray["kind"]);
            Assert.IsTrue(collaboratorVariableArray.ContainsKey("type"));

            var arrayType = collaboratorVariableArray["type"] as Dictionary<string, object>;
            Assert.IsNotNull(arrayType);

            Assert.AreEqual("Struct", arrayType["kind"]);
            Assert.AreEqual(string.Empty, arrayType["type"]);
            Assert.AreEqual("A.9d21537544d9123d.Momentables.Collaborator", arrayType["typeID"]);

            var structFields = arrayType["fields"] as List<object>;
            Assert.IsNotNull(structFields);

            var collaboratorWallet = structFields[1] as Dictionary<string, object>;
            Assert.IsNotNull(collaboratorWallet);

            Assert.AreEqual("collaboratorWallet", collaboratorWallet["id"]);

            var collaboratorWalletType = collaboratorWallet["type"] as Dictionary<string, object>;
            Assert.IsNotNull(collaboratorWalletType);

            Assert.AreEqual("Capability", collaboratorWalletType["kind"]);

            var capabilityType = collaboratorWalletType["type"] as Dictionary<string, object>;

            Assert.AreEqual(false, capabilityType["authorized"]);
            Assert.AreEqual("Reference", capabilityType["kind"]);

            var referenceType = capabilityType["type"] as Dictionary<string, object>;
            Assert.IsNotNull(referenceType);

            //we finally made it to the restricted type that has the repeated type!!
            Assert.AreEqual("Restriction", referenceType["kind"]);
            Assert.AreEqual("AnyResource{A.f233dcee88fe0abe.FungibleToken.Receiver}", referenceType["typeID"]);

            var restrictions = referenceType["restrictions"] as List<object>;
            Assert.AreEqual(1, restrictions.Count);
            Assert.AreEqual("A.f233dcee88fe0abe.FungibleToken.Receiver", restrictions.First());

            var innerType = referenceType["type"] as Dictionary<string, object>;
            Assert.IsNotNull(innerType);
            Assert.AreEqual("AnyResource", innerType["kind"]);
        }

        [TestMethod]
        public async Task LegacyTypeJson_DeserializesCorrectly()
        {
            var res = await GetTransaction(24684541, "c6043a12ddab4740d6dbb27a9171062813b0fff05f0a03529c61c620311be8e4", NodeType.MainNet);

            var ev = res.Events[0]; //A.4eb8a10cb9f87357.NFTStorefront.ListingAvailable

            var compositeData = ev.EventComposite.Data;

            //prior to secure cadence Types are just strings
            Assert.AreEqual("A.e4cf4bdc1751c65d.AllDay.NFT", compositeData["nftType"]);
            Assert.AreEqual("A.ead892083b3e2c6c.DapperUtilityCoin.Vault", compositeData["ftVaultType"]);
        }

        [TestMethod]
        public async Task String_With_NewLines()
        {
            var res = await GetTransaction(38393502, "8b80c75b1ebf03ef937988970964a9f981615863ada7c0a96f5b25069401f6d4", NodeType.MainNet);

            var evs = res.Events;

            var ticketDeposited = evs[6].EventComposite;
            var description = ticketDeposited.Data["description"];
            Assert.AreEqual("Appearance: The xG Reward for players with game time in a fixture.\n\nGet xG Rewards for your football achievements.\nBuild your collection - your story.\nUnlock xG experiences.\n\nhttps://linktr.ee/xgstudios", description);
        }

        [TestMethod]
        public async Task Array_WithDictionary()
        {
            var res = await GetTransaction(39272677, "1a9ec1df4c7fecb695ab536bb1d2f776c625a5855f21f37c952819418151a517", NodeType.MainNet);

            var evs = res.Events;
            var purchaseDetails = evs[4];

            var composite = purchaseDetails.EventComposite;
            var arr = composite.Data["momentsInPack"] as List<object>;

            //this array contains dictionaries lol
            var dict = arr[0] as Dictionary<string, object>;
            Assert.IsNotNull(dict);

            Assert.IsTrue(dict.ContainsKey("id"));
            var id = Convert.ToInt64(dict["id"]);
            Assert.AreEqual(7689m, id);

            Assert.IsTrue(dict.ContainsKey("serial"));
            var serial = Convert.ToInt64(dict["serial"]);
            Assert.AreEqual(394m, serial);
        }

        [TestMethod]
        public async Task OptionalString_ContainsEscapedCharacters()
        {
            var res = await GetTransaction(40209600, "60b8d95c0656da483e879484fa57017f1b1a8e3e0c24ee59c4d7eda75ca03f1d", NodeType.MainNet);

            var evs = res.Events;
            var ev = evs[2];
            var data = ev.EventComposite.Data;

            var description = data["description"] as string;
            Assert.IsNotNull(description);

            //https://flowscan.org/transaction/60b8d95c0656da483e879484fa57017f1b1a8e3e0c24ee59c4d7eda75ca03f1d
            Assert.AreEqual("Derrick \"The Black Beast\" Lewis delivers a lights out uppercut against Curtis Blaydes, to break the record for the most KOs in UFC heavyweight history and tie for the most KO/TKO wins in UFC history", description);
        }

        [TestMethod]
        public async Task AccountCreated()
        {
            //starting in testnet 39 the sequence of individual json members is not guaranteed
            var res = await GetTransaction(51356003, "01c309b3ad103218dc5a3010cc9430ac1ce7ea47f9ea156095dd10485aab4317", NodeType.MainNet);

            var evs = res.Events;
            var accountCreated = evs[5];

            var data = accountCreated.EventComposite.Data;
            Assert.IsTrue(data.ContainsKey("address"));

            Assert.IsInstanceOfType<string>(data["address"]);
            Assert.AreEqual("0x18f951de04917a5b", data["address"] as string);
        }

        [TestMethod]
        public async Task FindMarketSale()
        {
            var res = await GetTransaction(78123765, "2debae8c8ab902cf41c886681d470ea97e4e9a64e9f9842d2369b4fd7fab982e", NodeType.MainNet);

            var evs = res.Events;
            var findMarketSale = evs[0].EventComposite.Data;

            //verify some primitive data
            Assert.AreEqual("find", findMarketSale["tenant"].ToString());
            Assert.AreEqual("182518930557703", findMarketSale["id"].ToString());
            Assert.AreEqual("111050675032800", findMarketSale["saleID"].ToString());
            Assert.AreEqual("0x8b988d0ce5d25a8c", findMarketSale["seller"]);

            //get some complex data from the event
            var nft = findMarketSale["nft"] as Dictionary<string, object>;

            Assert.AreEqual("182518930557703", nft["id"].ToString());
            Assert.AreEqual("Holder", nft["name"]);
        }

        [TestMethod]
        public async Task NFTStoreFrontV2_ListingCompleted()
        {
            var res = await GetTransaction(78686132, "a2cac27776ba306e7a817ba9b3310efb08554acb6619cb9c8cca3f12aef0a6f9", NodeType.MainNet);

            var evs = res.Events;
            var listingCompleted = evs[0].EventComposite.Data;

            //verify some primitive data
            Assert.AreEqual("156130651796375", listingCompleted["listingResourceID"].ToString());
            Assert.AreEqual("994782813", listingCompleted["storefrontResourceID"].ToString());
            Assert.AreEqual("False", listingCompleted["purchased"].ToString());

            //verify some complex data
            var nftType = listingCompleted["nftType"] as Dictionary<string, object>;
            Assert.AreEqual("Resource", nftType["kind"]);
            Assert.AreEqual("A.d0bcefdf1e67ea85.HWGarageCard.NFT", nftType["typeID"]);
        }

        private async Task<FlowTransactionResult> GetTransaction(ulong blockHeight, string transactionId, NodeType nodeType)
        {
            var flowClient = nodeType switch
            {
                NodeType.MainNet => _main.CreateFlowClient(blockHeight),
                NodeType.TestNet => _test.CreateFlowClient(blockHeight), //Flow does not retain old testnet data, refrain from writing tests against testnet transactions
                _ => throw new Exception()
            };
            return await flowClient.GetTransactionResult(transactionId.HashToByteString());
        }
    }
}