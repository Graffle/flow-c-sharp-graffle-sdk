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
    [TestClass]
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
            var collectionId = block.RawBlock.CollectionGuarantees.FirstOrDefault().CollectionId;
            var collection = await flowClient.GetCollectionById(collectionId);
            var transactionId = collection.TransactionIds.FirstOrDefault();
            var thing = transactionId.ByteStringToHex();
            var transactionResult = await flowClient.GetTransactionResult(transactionId);
            var transaction = await flowClient.GetTransactionAsync(transactionId);
            var complete = await flowClient.GetCompleteTransactionAsync(transactionId);
             var result = string.Empty;
        }
    }
}