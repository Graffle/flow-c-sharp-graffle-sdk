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
        [TestMethod]
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

        [TestMethod]
        [Ignore] //TODO backwards compabilitility - this txn has the old json structure for Type
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
        public async Task Serialize_SecureCadenceNewStructJson_Succeeds()
        {
            var res = await GetTransaction(70622950, "1889e0548b9b9486721d581b5bf6b5665a0b714ed4dc6d5e9fd8d1cae676d5da");
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
            Assert.AreEqual("A.ff68241f0f4fd521.DrSeuss.NFT", dict["typeID"]);

            Assert.IsTrue(dict.ContainsKey("initializers"));
            var initializers = dict["initializers"];

            Assert.IsInstanceOfType(initializers, typeof(List<Dictionary<string, object>>));
            var initList = initializers as List<Dictionary<string, object>>;
            Assert.IsTrue(initList.Count == 0);

            Assert.IsTrue(dict.ContainsKey("fields"));
            var fields = dict["fields"];
            Assert.IsInstanceOfType(fields, typeof(List<Dictionary<string, object>>));

            var fieldList = fields as List<Dictionary<string, dynamic>>;

            Assert.AreEqual(5, fieldList.Count());

            //pull out an item

            var item = fieldList[2];

            Assert.IsTrue(item.ContainsKey("id"));
            Assert.AreEqual("mintNumber", item["id"]);

            Assert.IsTrue(item.ContainsKey("type"));

            var itemType = item["type"];
            Assert.IsInstanceOfType(itemType, typeof(Dictionary<string, object>));

            var itemTypeDict = itemType as Dictionary<string, object>;
            Assert.IsTrue(itemTypeDict.ContainsKey("kind"));

            Assert.AreEqual(itemTypeDict["kind"], "UInt32");
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

            var fieldsList = fields as List<Dictionary<string, object>>;

            var dictionaryType = fieldsList[4];

            var dictionaryDict = dictionaryType as Dictionary<string, object>;

            Assert.AreEqual("metadata", dictionaryDict["id"]);

            var typeDict = dictionaryDict["type"] as Dictionary<string, object>;

            var key = typeDict["key"];

            var keyDict = key as Dictionary<string, object>;
            Assert.AreEqual("String", keyDict["kind"]);
        }

        private async Task<FlowTransactionResult> GetTransaction(ulong blockHeight, string transactionId, NodeType nodeType = NodeType.TestNet)
        {
            //probably don't need all of these calls but lets do them anyways to ensure no exceptions are thrown
            var flowClientFactory = new FlowClientFactory(nodeType);
            var flowClient = flowClientFactory.CreateFlowClient(blockHeight);
            var latestBlockResponse = await flowClient.GetLatestBlockAsync(true);
            var block = await flowClient.GetBlockByHeightAsync(blockHeight);

            var collectionId = block.CollectionGuarantees.FirstOrDefault()?.CollectionId;
            var collection = collectionId != null ? await flowClient.GetCollectionById(collectionId.HashToByteString()) : null;

            var transactionResult = await flowClient.GetTransactionResult(transactionId.HashToByteString());
            var transaction = await flowClient.GetTransactionAsync(transactionId.HashToByteString());
            var complete = await flowClient.GetCompleteTransactionAsync(transactionId.HashToByteString());

            return transactionResult;
        }
    }
}