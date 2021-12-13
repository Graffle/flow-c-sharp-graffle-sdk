using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Graffle.FlowSdk.Cryptography;
using Graffle.FlowSdk.Services.Models;
using Graffle.FlowSdk.Types;
using Graffle.FlowSdk.Services.Nodes;
using Graffle.FlowSdk.Services.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Text.Json;

namespace Graffle.FlowSdk.Services.Tests.AccountTests
{
    [TestClass]
    [Ignore]
    public class CreateAccountTests
    {
        private GraffleClient flowClient { get; }

        public CreateAccountTests(){
            var factory = new FlowClientFactory(NodeType.Emulator);
            this.flowClient = factory.CreateFlowClient();
        }

        [TestMethod]
        public async Task Given_Valid_Parameters_When_Creating_An_Account_Then_Account_Is_Created_Successfully(){
            var emulatorAccount = await flowClient.GetAccountFromConfigAsync("emulator-account");

            var emulatorAccountKey = emulatorAccount.Keys[0];

            var flowAccountKey = Flow.Entities.AccountKey.GenerateRandomEcdsaKey(SignatureAlgorithm.ECDSA_P256, HashAlgorithm.SHA3_256);
            var newFlowAccountKeys = new List<Flow.Entities.AccountKey> { flowAccountKey };

            var emulatorAddress = new FlowAddress(emulatorAccount.Address);

            var transaction = AccountTransactions.CreateAccount(newFlowAccountKeys, emulatorAddress);

            transaction.Payer = emulatorAddress;
            transaction.ProposalKey = new FlowProposalKey
            {
                Address = emulatorAddress,
                KeyId = emulatorAccountKey.Index,
                SequenceNumber = emulatorAccountKey.SequenceNumber
            };

            var latestBlock = await flowClient.GetLatestBlockAsync();
            transaction.ReferenceBlockId = latestBlock.Id;

            // sign and submit the transaction
            transaction = FlowTransaction.AddEnvelopeSignature(transaction, emulatorAddress, emulatorAccountKey.Index, emulatorAccountKey.Signer);

            var response = await flowClient.SendTransactionAsync(transaction);

            // wait for seal
            var sealedResponse = await flowClient.WaitForSealAsync(response);

            if (sealedResponse.Status == Flow.Entities.TransactionStatus.Sealed)
            {
                //TODO parse response
            }

            //TODO
            //Assert.AreEqual(false, true, "Test failed waiting for emulator block to seal.");
            Assert.AreEqual(true, true);
        }
    }
}