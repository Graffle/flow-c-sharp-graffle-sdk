using System.Collections.Generic;
using System.Threading.Tasks;
using Graffle.FlowSdk.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Text.Json;
using Graffle.FlowSdk.Services.Nodes;
using System.Linq;
using Graffle.FlowSdk.Services.Models;

namespace Graffle.FlowSdk.Services.Tests.TransactionsTests
{
    //Quick internal test.
    //TODO: Update to real automated tests.
    [TestClass]
    [Ignore]
    public class TransactionResponsesTests
    {
        public TransactionResponsesTests()
        {
        }

        [TestMethod]
        public async Task Test_Test()
        {
            var flowClientFactory = new FlowClientFactory(NodeType.MainNet);
            var flowClient = flowClientFactory.CreateFlowClient(21762224);
            var latestBlockResponse = await flowClient.GetLatestBlockAsync(true);

            var block = await flowClient.GetBlockByHeightAsync(21762224);
            var collectionId = block.CollectionGuarantees.FirstOrDefault().CollectionId;
            var collection = await flowClient.GetCollectionById(collectionId.HashToByteString());
            var transactionId = collection.TransactionIds.FirstOrDefault();
            var transactionResult = await flowClient.GetTransactionResult(transactionId.HashToByteString());
            var transaction = await flowClient.GetTransactionAsync(transactionId.HashToByteString());
            var complete = await flowClient.GetCompleteTransactionAsync(transactionId.HashToByteString());
            var result = string.Empty;
        }
    }
}