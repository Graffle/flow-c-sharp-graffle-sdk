using Graffle.FlowSdk.Services.Models;
using Graffle.FlowSdk.Services.Nodes;
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
        private static IFlowClientFactory _main;
        private static IFlowClientFactory _test;

        [ClassInitialize]
        public static void ClassInit(TestContext ctx)
        {
            _main = new FlowClientFactory("MainNet") { UseBetaDeserializer = true };
            _test = new FlowClientFactory("TestNet") { UseBetaDeserializer = true };
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _main?.Dispose();
            _test?.Dispose();
        }

        [TestMethod]
        [Ignore] //todo refactor
        public async Task TransactionWithArray()
        {
            var transactionResult = await GetTransaction(60145148, "35a060e0a370220ad0c949852afcd88da8a965e2bc829b332f512f7618ecedfc");

            var events = transactionResult.Events;

            //get the json we want to verify from the event payload
            var eventWithArray = events.FirstOrDefault(ev => ev.Type == "A.056a9cc93a020fad.DimeStorefrontV2.SaleOfferAdded");
            var payload = eventWithArray.Payload;
            var parsedJson = JsonDocument.Parse(payload);
            var valueRoot = parsedJson.RootElement.GetProperty("value");
            var fieldsRoot = valueRoot.GetProperty("fields");
            var myField = fieldsRoot.EnumerateArray().Skip(1).FirstOrDefault();
            var array = myField.GetProperty("value");
            var json = array.GetRawText();

            //use our sdk to parse the json and extract information for verification
            var parsedType = FlowValueType.CreateFromCadence(json);
            Assert.IsNotNull(parsedType);
            Assert.IsInstanceOfType(parsedType, typeof(ArrayType));
            var arrayType = parsedType as ArrayType;

            var data = arrayType.Data;
            Assert.AreEqual(1, data.Count);

            var item = data[0];
            Assert.IsNotNull(item);
            Assert.IsInstanceOfType(item, typeof(AddressType));
            var addressType = item as AddressType;

            Assert.AreEqual(addressType.Data, "0x26a608ed644c844f");
        }

        [TestMethod]
        [Ignore] //todo refactor
        public async Task Transaction_Dictionary_Contains_Structs()
        {
            var transactionResult = await GetTransaction(60147017, "2d724581f3758d9515951c585ee7795e5e22755f81d4268688a601c87ec8fbd7");

            var events = transactionResult.Events;

            //get the json we want to verify from the event payload
            var eventWithArray = events.FirstOrDefault(ev => ev.Type == "A.2a37a78609bba037.TheFabricantS1ItemNFT.ItemDataCreated");
            var payload = eventWithArray.Payload;
            var parsedJson = JsonDocument.Parse(payload);
            var valueRoot = parsedJson.RootElement.GetProperty("value");
            var fieldsRoot = valueRoot.GetProperty("fields");
            var myField = fieldsRoot.EnumerateArray().Skip(2).FirstOrDefault();
            var dictionary = myField.GetProperty("value");
            var json = dictionary.GetRawText();

            //use our sdk to parse the json and extract information for verification
            var parsedType = FlowValueType.CreateFromCadence(json);
            Assert.IsNotNull(parsedType);
            Assert.IsInstanceOfType(parsedType, typeof(DictionaryType));

            var dictionaryType = parsedType as DictionaryType;
            var data = dictionaryType.Data;

            Assert.AreEqual(8, data.Keys.Count);

            //let's pull out a single struct and validate it
            var itemToTest = data.Where(kvp =>
            {
                var key = kvp.Key;
                if (key is StringType s && s.Data == "itemImage4")
                    return true;

                return false;
            }).FirstOrDefault();

            Assert.IsNotNull(itemToTest);
            Assert.IsNotNull(itemToTest.Value);
            Assert.IsInstanceOfType(itemToTest.Value, typeof(CompositeType));

            var structToTest = itemToTest.Value as CompositeType;
            Assert.AreEqual("A.2a37a78609bba037.TheFabricantS1ItemNFT.Metadata", structToTest.Id);
            Assert.AreEqual(2, structToTest.Fields.Count);

            //verify each item
            var first = structToTest.Fields[0];
            Assert.AreEqual("metadataValue", first.Name);
            var firstValue = first.Value;
            Assert.IsNotNull(firstValue);
            Assert.IsInstanceOfType(firstValue, typeof(StringType));
            var tmp = firstValue as StringType;
            Assert.AreEqual("", tmp.Data);

            var second = structToTest.Fields[1];
            Assert.AreEqual("mutable", second.Name);
            var secondValue = second.Value;
            Assert.IsNotNull(secondValue);
            Assert.IsInstanceOfType(secondValue, typeof(BoolType));
            var tmp2 = secondValue as BoolType;
            Assert.AreEqual(true, tmp2.Data);

            //let's do some verification on the grafflecomposite
            var graffleComposite = eventWithArray.EventComposite;

            //pull out the dictionary full of structs
            var dict = graffleComposite.Data["metadatas"];
            Assert.AreEqual(8, dict.Count);

            //lets check out a single struct in here
            var fields = dict["itemVideo"];
            Assert.IsInstanceOfType(fields, typeof(Dictionary<string, object>));
            Assert.AreEqual(2, fields.Count);
        }

        [TestMethod]
        [Ignore] //todo refactor
        public async Task Transaction_Dictionary_With_UInt64Key()
        {
            var transactionResult = await GetTransaction(59886127, "bf49159ac950b2a147eb8df651c1bb125e6c1b202de73ace85d17986a70df9ca");

            var events = transactionResult.Events;

            //get the json we want to verify from the event payload
            var eventWithArray = events.FirstOrDefault(ev => ev.Type == "A.a4df2ca3e19f32ef.NftItems.Minted");
            var payload = eventWithArray.Payload;
            var parsedJson = JsonDocument.Parse(payload);
            var valueRoot = parsedJson.RootElement.GetProperty("value");
            var fieldsRoot = valueRoot.GetProperty("fields");
            var myField = fieldsRoot.EnumerateArray().Skip(1).FirstOrDefault();
            var dictionary = myField.GetProperty("value");
            var json = dictionary.GetRawText();

            //use our sdk to parse the json and extract information for verification
            var parsedType = FlowValueType.CreateFromCadence(json);
            Assert.IsNotNull(parsedType);
            Assert.IsInstanceOfType(parsedType, typeof(DictionaryType));

            var dict = parsedType as DictionaryType;
            var data = dict.Data;
            Assert.AreEqual(1, data.Keys.Count);

            var key = data.Keys.First();
            Assert.IsInstanceOfType(key, typeof(UInt64Type));
            var intKey = key as UInt64Type;
            Assert.AreEqual((ulong)1, intKey.Data);

            var value = data[key];
            Assert.IsNotNull(value);
            Assert.IsInstanceOfType(value, typeof(DictionaryType));
            var valueDict = value as DictionaryType;

            //this inner dictionary has string keys so not going to do much verification here
            Assert.AreEqual(3, valueDict.Data.Keys.Count);
        }

        [TestMethod]
        [Ignore] //todo refactor
        public async Task Transaction_StructContainsOptionalStruct()
        {
            var res = await GetTransaction(60816404, "4c6e308af1982760a269c2e44a62dfbea77b62fc36bbacf5e35142ae235dc743");
            var events = res.Events;
            var ev = events.FirstOrDefault(ev => ev.Type == "A.76b2527585e45db4.SoulMadeMarketplace.SoulMadeForSale");
            var payload = ev.Payload;
            var parsedJson = JsonDocument.Parse(payload);
            var valueRoot = parsedJson.RootElement.GetProperty("value");
            var fieldsRoot = valueRoot.GetProperty("fields");
            var myField = fieldsRoot.EnumerateArray().Skip(3).FirstOrDefault();
            var composite = myField.GetProperty("value");
            var json = composite.GetRawText();

            var parsed = FlowValueType.CreateFromCadence(json);
            Assert.IsInstanceOfType(parsed, typeof(CompositeType));
            var flowStruct = parsed as CompositeType;

            //top level struct
            //validate fields and id
            Assert.AreEqual("A.76b2527585e45db4.SoulMadeMarketplace.SoulMadeSaleData", flowStruct.Id);
            Assert.AreEqual(5, flowStruct.Fields.Count);

            //the last field is an Optional field containing a Struct
            var optionalStructField = flowStruct.Fields.Last();
            Assert.AreEqual("componentDetail", optionalStructField.Name);

            //verify it's OptionalType
            var optionalStructFieldValue = optionalStructField.Value;
            Assert.IsInstanceOfType(optionalStructFieldValue, typeof(OptionalType));

            //verify the OptionalType contains a Struct Type
            var optional = optionalStructFieldValue as OptionalType;
            Assert.IsNotNull(optional.Data);
            Assert.IsInstanceOfType(optional.Data, typeof(CompositeType));

            //validate inner struct
            var innerStruct = optional.Data as CompositeType;
            Assert.AreEqual("A.76b2527585e45db4.SoulMadeComponent.ComponentDetail", innerStruct.Id);
            Assert.AreEqual(10, innerStruct.Fields.Count);

            //let's validate the grafflecomposite
            var graffleComposite = ev.EventComposite;
            var data = graffleComposite.Data;
            Assert.IsNotNull(data);
            Assert.AreEqual(4, data.Count);

            //pull out the field with a nested struct
            var nestedFields = data["saleData"]["componentDetail"];
            Assert.IsNotNull(nestedFields);
            Assert.IsInstanceOfType(nestedFields, typeof(Dictionary<string, object>));
            Assert.AreEqual(10, nestedFields.Count); //10 fields in the nested struct
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
        [Ignore] //todo
        public async Task Serialize_LargeTransaction_Succeeds()
        {
            var res = await GetTransaction(25568454, "64a8781b0c6873d98ec30d5ef6ee296dcdddf93a8c2ec2e4378a6cfaea6b2631", NodeType.MainNet);
            var events = res.Events;

            Assert.AreEqual(29625, events.Count());
        }

        [TestMethod]
        [Ignore] //todo
        public async Task Serialize_ArrayWithStructs_Succeeds()
        {
            var res = await GetTransaction(67676278, "94c061a2075679cf8df22bab85f2979739921a0c64939ce7ae1036629b55eaff");
            var events = res.Events;
            var ev = events[2];

            var composite = ev.EventComposite;

            Assert.IsNotNull(composite);

            //this object is very complex and contains many nested complex types
            //lets recurse through the data and touch some stuff to make sure it's all there
            var data = composite.Data;
            Assert.AreEqual(4, data.Keys.Count());

            if (!data.TryGetValue("saleData", out dynamic saleData))
            {
                Assert.Fail("saleData not found in dictionary");
            }

            //saleData is a struct
            Assert.IsInstanceOfType(saleData, typeof(Dictionary<string, object>));
            var saleDataDict = saleData as Dictionary<string, object>;

            //maindetail is also a struct
            var mainDetail = saleDataDict["mainDetail"];
            Assert.IsInstanceOfType(mainDetail, typeof(Dictionary<string, object>));
            var mainDetailDict = mainDetail as Dictionary<string, object>;

            //component details is an array
            var componentDetails = mainDetailDict["componentDetails"];
            Assert.IsInstanceOfType(componentDetails, typeof(List<object>));
            var componentDetailsList = componentDetails as List<object>;

            Assert.AreEqual(11, componentDetailsList.Count);

            //this array contains structs
            //lets just verify one
            var item = componentDetailsList[0];
            Assert.IsInstanceOfType(item, typeof(Dictionary<string, object>));

            var itemDict = item as Dictionary<string, object>;
            Assert.AreEqual(9, itemDict.Count());

            //verify fields for this struct
            if (!itemDict.TryGetValue("id", out object id))
            {
                Assert.Fail("id not found in dictionary");
            }

            Assert.AreEqual((UInt64)107520, id);

            if (!itemDict.TryGetValue("name", out object name))
            {
                Assert.Fail("name not found");
            }

            Assert.AreEqual("Victorian Dream", name);

            if (!itemDict.TryGetValue("series", out object series))
            {
                Assert.Fail("series not found");
            }

            Assert.AreEqual("Disordered-FengFeng", series);
        }

        [TestMethod]
        [Ignore] //todo refactor
        public async Task Serialize_SecureCadenceNewStructJson_Succeeds()
        {
            var res = await GetTransaction(96377682, "f1a275a477d150bc89b88ee1c3a2af957013c795be669cae8cdb56e32c6176c2");
            var events = res.Events;
            var nftStoreFront = events[0];

            var composite = nftStoreFront.EventComposite;

            //get the type field
            var nftType = composite.Data["nftType"];
            Assert.IsInstanceOfType(nftType, typeof(Dictionary<string, object>));
            var dict = nftType as Dictionary<string, object>;

            //verify properties
            Assert.IsTrue(dict.ContainsKey("kind"));
            Assert.AreEqual("Resource", dict["kind"]);

            Assert.IsTrue(dict.ContainsKey("type"));
            Assert.AreEqual(string.Empty, dict["type"]);

            Assert.IsTrue(dict.ContainsKey("typeID"));
            Assert.AreEqual("A.4dfd62c88d1b6462.AllDay.NFT", dict["typeID"]);

            Assert.IsTrue(dict.ContainsKey("initializers"));
            var initializers = dict["initializers"];

            Assert.IsInstanceOfType(initializers, typeof(List<object>));
            var initList = initializers as List<object>;
            Assert.IsTrue(initList.Count == 0);

            Assert.IsTrue(dict.ContainsKey("fields"));
            var fields = dict["fields"];
            Assert.IsInstanceOfType(fields, typeof(List<object>));

            var fieldList = fields as List<object>;

            Assert.AreEqual(5, fieldList.Count());

            //pull out an item

            var item = fieldList[2] as Dictionary<string, object>;
            Assert.IsNotNull(item);

            Assert.IsTrue(item.ContainsKey("id"));
            Assert.AreEqual("editionID", item["id"]);

            Assert.IsTrue(item.ContainsKey("type"));

            var itemType = item["type"];
            Assert.IsInstanceOfType(itemType, typeof(Dictionary<string, object>));

            var itemTypeDict = itemType as Dictionary<string, object>;
            Assert.IsTrue(itemTypeDict.ContainsKey("kind"));

            Assert.AreEqual(itemTypeDict["kind"], "UInt64");
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
        [Ignore] //todo refactor
        public async Task OptionalStruct_ContainsOptionalTypes()
        {
            var res = await GetTransaction(96364363, "07dd1909a0bc732cc9092010588571fab617b7a7c573058741e1f0391c74551c");

            var ev = res.Events[0];

            var composite = ev.EventComposite;

            var nft = composite.Data["nft"] as Dictionary<string, object>;
            Assert.IsNotNull(nft);

            var collectionName = nft["collectionName"];
            var collectionDescription = nft["collectionDescription"];

            Assert.AreEqual("Wearables", collectionName);
            Assert.AreEqual("Doodles 2 lets anyone create a uniquely personalized and endlessly customizable character in a one-of-a-kind style. Wearables and other collectibles can easily be bought, traded, or sold. Doodles 2 will also incorporate collaborative releases with top brands in fashion, music, sports, gaming, and more.\n\nDoodles 2 Private Beta, which will offer first access to the Doodles character creator tools, will launch later in 2022. Doodles 2 Private Beta will only be available to Beta Pass holders.", collectionDescription);
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
        //    [Ignore] //todo refactor
        public async Task TestNet39_AccountCreated()
        {
            //starting in testnet 39 the sequence of individual json members is not guaranteed
            var res = await GetTransaction(177644105, "2201cc03967515ec9b335ba78da3853b107c2547a1bb49b309b24b7a8b90b6fd");

            var evs = res.Events;
            var accountCreated = evs[5];

            var data = accountCreated.EventComposite.Data;
            Assert.IsTrue(data.ContainsKey("address"));

            Assert.IsInstanceOfType<string>(data["address"]);
            Assert.AreEqual("0x5cfdc888e006eb85", data["address"] as string);
        }

        [TestMethod]
        [Ignore] //todo refactor
        public async Task TestNet39_FindMarketSale()
        {
            var res = await GetTransaction(90623401, "01140138555b59a8c83d4201d7ea42234476c6347724db60f0d614f813a66a6a");

            var evs = res.Events;
            var findMarketSale = evs[1].EventComposite.Data;

            //verify some primitive data
            Assert.AreEqual("onefootball", findMarketSale["tenant"].ToString());
            Assert.AreEqual("112985363", findMarketSale["id"].ToString());
            Assert.AreEqual("119660397", findMarketSale["saleID"].ToString());
            Assert.AreEqual("0xc93b666cb28bf15a", findMarketSale["seller"]);

            //get some complex data from the event
            var nft = findMarketSale["nft"] as Dictionary<string, object>;

            Assert.AreEqual("112985363", nft["id"].ToString());
            Assert.AreEqual("Meet Juventus’ Secretary of Defense", nft["name"]);
        }

        [TestMethod]
        [Ignore] //todo refactor
        public async Task TestNet39_NFTStoreFrontV2_ListingCompleted()
        {
            var res = await GetTransaction(90639372, "d9968ffbf85ac3c9b73052e113c476585348352521c923b0b01d8ed3044cb6f2");

            var evs = res.Events;
            var listingCompleted = evs[13].EventComposite.Data;

            //verify some primitive data
            Assert.AreEqual("124595091", listingCompleted["listingResourceID"].ToString());
            Assert.AreEqual("123164895", listingCompleted["storefrontResourceID"].ToString());
            Assert.AreEqual("True", listingCompleted["purchased"].ToString());

            //verify some complex data
            var nftType = listingCompleted["nftType"] as Dictionary<string, object>;
            Assert.AreEqual("Resource", nftType["kind"]);
            Assert.AreEqual("A.2ba03636e5c3e411.Magnetiq.NFT", nftType["typeID"]);
        }

        [TestMethod]
        [Ignore] //todo refactor
        public async Task OptionalArray()
        {
            var res = await GetTransaction(96370355, "87fd126ccd565384f1b738d52a4334161e99e05e8aa016070050fd575403b59c");

            var evs = res.Events;

            var assetUpdated = evs[0].EventComposite.Data;

            Assert.IsTrue(assetUpdated.ContainsKey("orh"));
            var orh = assetUpdated["orh"] as List<object>;
            Assert.IsNotNull(orh);

            var asString = orh.Select(x => x.ToString()).ToHashSet();
            Assert.IsTrue(asString.Contains("Gino"));
            Assert.IsTrue(asString.Contains("Mario"));
            Assert.IsTrue(asString.Contains("Soul"));
            Assert.IsTrue(asString.Contains("Tom"));
            Assert.IsTrue(asString.Contains("Sue"));
            Assert.IsTrue(asString.Contains("Kim"));
            Assert.IsTrue(asString.Contains("Toto"));

            Assert.AreEqual(7, asString.Count);
        }

        private async Task<FlowTransactionResult> GetTransaction(ulong blockHeight, string transactionId, NodeType nodeType = NodeType.TestNet)
        {
            var flowClient = nodeType switch
            {
                NodeType.MainNet => _main.CreateFlowClient(blockHeight),
                NodeType.TestNet => _test.CreateFlowClient(blockHeight),
                _ => throw new Exception()
            };

            var transactionResult = await flowClient.GetTransactionResult(transactionId.HashToByteString());
            return transactionResult;
        }
    }
}