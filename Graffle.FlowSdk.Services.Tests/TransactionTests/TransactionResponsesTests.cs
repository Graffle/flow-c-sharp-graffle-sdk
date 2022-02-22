using Graffle.FlowSdk.Services.Models;
using Graffle.FlowSdk.Services.Nodes;
using Graffle.FlowSdk.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            Assert.IsInstanceOfType(itemToTest.Value, typeof(StructType));

            var structToTest = itemToTest.Value as StructType;
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
            var structType = myField.GetProperty("value");
            var json = structType.GetRawText();

            var parsed = FlowValueType.CreateFromCadence(json);
            Assert.IsInstanceOfType(parsed, typeof(StructType));
            var flowStruct = parsed as StructType;

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
            Assert.IsInstanceOfType(optional.Data, typeof(StructType));

            //validate inner struct
            var innerStruct = optional.Data as StructType;
            Assert.AreEqual("A.76b2527585e45db4.SoulMadeComponent.ComponentDetail", innerStruct.Id);
            Assert.AreEqual(10, innerStruct.Fields.Count);
        }

        private async Task<FlowTransactionResult> GetTransaction(ulong blockHeight, string transactionId, NodeType nodeType = NodeType.TestNet)
        {
            //probably don't need all of these calls but lets do them anyways to ensure no exceptions are thrown
            var flowClientFactory = new FlowClientFactory(nodeType);
            var flowClient = flowClientFactory.CreateFlowClient(blockHeight);
            var latestBlockResponse = await flowClient.GetLatestBlockAsync(true);
            var block = await flowClient.GetBlockByHeightAsync(blockHeight);
            var collectionId = block.CollectionGuarantees.FirstOrDefault().CollectionId;
            var collection = await flowClient.GetCollectionById(collectionId.HashToByteString());
            var transactionResult = await flowClient.GetTransactionResult(transactionId.HashToByteString());
            var transaction = await flowClient.GetTransactionAsync(transactionId.HashToByteString());
            var complete = await flowClient.GetCompleteTransactionAsync(transactionId.HashToByteString());

            return transactionResult;
        }
    }
}