using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Graffle.FlowSdk.Cryptography;
using Graffle.FlowSdk.Services.Models;
using Graffle.FlowSdk.Services.Nodes;
using Graffle.FlowSdk.Services.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graffle.FlowSdk.Services.Tests.TransactionsTests
{
    [TestClass]
    [Ignore]
    public class TransactionSendTests
    {
        private IGraffleClient flowClient { get; }

        public TransactionSendTests(){
            var factory = new FlowClientFactory(NodeType.Emulator);
            this.flowClient = factory.CreateFlowClient();
        }

        [TestMethod]
        public async Task Given_Valid_Parameters_When_Sending_Empty_Transaction_Then_Transaction_Is_Successful(){
            var emulatorAccount = await flowClient.GetAccountFromConfigAsync("emulator-account");
            var emulatorAddress = new FlowAddress(emulatorAccount.Address);
            var emulatorAccountKey = emulatorAccount.Keys[0];

            var latestBlock = await flowClient.GetLatestBlockAsync();
            var transaction = new FlowTransaction
            {
                Script = new FlowScript("transaction {}"),
                ReferenceBlockId = latestBlock.Id,
                Payer = emulatorAddress,
                ProposalKey = new FlowProposalKey
                {
                    Address = emulatorAddress,
                    KeyId = emulatorAccountKey.Index,
                    SequenceNumber = emulatorAccountKey.SequenceNumber
                }
            };

            transaction = FlowTransaction.AddEnvelopeSignature(transaction, emulatorAddress, emulatorAccountKey.Index, emulatorAccountKey.Signer);

            var response = await flowClient.SendTransactionAsync(transaction);

           // wait for seal
            var sealedResponse = await flowClient.WaitForSealAsync(response);
            
            Assert.AreEqual(Flow.Entities.TransactionStatus.Sealed, sealedResponse.Status, "Test failed waiting for emulator block to seal.");
            Assert.AreEqual(0, sealedResponse.Events.Count);

            var sealedTransaction = await flowClient.GetTransactionAsync(response.Id);
            Assert.AreEqual(0, sealedTransaction.Arguments.Count);
            Assert.AreEqual(emulatorAddress.HexValue, sealedTransaction.Payer.HexValue);
            Assert.AreEqual(0, sealedTransaction.Authorizers.Count);
        }
        
        [TestMethod]
        public async Task Given_Valid_Parameters_When_Signing_Transaction_With_Multiple_Signatures_Then_Transaction_Is_Successful(){
            var flowAccountKeyA = Flow.Entities.AccountKey.GenerateRandomEcdsaKey(SignatureAlgorithm.ECDSA_P256, HashAlgorithm.SHA3_256);
            var newFlowAccountKeysA = new List<Flow.Entities.AccountKey> { flowAccountKeyA };
            var accountA = await CreateFlowAccountAsync(newFlowAccountKeysA);
            var accountAKey = accountA.Keys.FirstOrDefault();
            var addressA = new FlowAddress(accountA.Address.HashToByteString());

            var flowAccountKeyB = Flow.Entities.AccountKey.GenerateRandomEcdsaKey(SignatureAlgorithm.ECDSA_P256, HashAlgorithm.SHA3_256);
            var newFlowAccountKeysB = new List<Flow.Entities.AccountKey> { flowAccountKeyB };
            var accountB = await CreateFlowAccountAsync(newFlowAccountKeysB);
            var accountBKey = accountB.Keys.FirstOrDefault();
            var addressB = new FlowAddress(accountB.Address.HashToByteString());

            var latestBlock = await flowClient.GetLatestBlockAsync();

            var transaction = new FlowTransaction
            {
                Script = new FlowScript("transaction {prepare(signer: AuthAccount) { log(signer.address) }}"),
                GasLimit = 9999,
                Payer = addressB,
                ProposalKey = new FlowProposalKey
                {
                    Address = addressA,
                    KeyId = accountAKey.Index,
                    SequenceNumber = accountAKey.SequenceNumber
                },
                ReferenceBlockId = latestBlock.Id
            };

            transaction.Authorizers.Add(addressA);

            transaction = FlowTransaction.AddPayloadSignature(transaction, addressA, accountAKey.Index, accountAKey.Signer);

            transaction = FlowTransaction.AddEnvelopeSignature(transaction, addressB, accountBKey.Index, accountBKey.Signer);

            var response = await flowClient.SendTransactionAsync(transaction);

            // wait for seal
            var sealedResponse = await flowClient.WaitForSealAsync(response);

            Assert.AreEqual(Flow.Entities.TransactionStatus.Sealed, sealedResponse.Status, "Test failed waiting for emulator block to seal.");
            Assert.AreEqual(0, sealedResponse.Events.Count);

            var sealedTransaction = await flowClient.GetTransactionAsync(response.Id);
            Assert.AreEqual(0, sealedTransaction.Arguments.Count);
            Assert.AreEqual(addressB.HexValue, sealedTransaction.Payer.HexValue);
            Assert.AreEqual(addressA.HexValue, sealedTransaction.Authorizers[0].HexValue);          
            Assert.AreEqual(addressA.HexValue, sealedTransaction.PayloadSignatures[0].Address.HexValue);
            Assert.AreEqual(addressB.HexValue, sealedTransaction.EnvelopeSignatures[0].Address.HexValue);
        }

        private async Task<FlowAccount> CreateFlowAccountAsync(List<Flow.Entities.AccountKey> newAccountKeys){
            var emulatorAccount = await flowClient.GetAccountFromConfigAsync("emulator-account");

            var emulatorAccountKey = emulatorAccount.Keys[0];

            var flowAccountKey = Flow.Entities.AccountKey.GenerateRandomEcdsaKey(SignatureAlgorithm.ECDSA_P256, HashAlgorithm.SHA3_256);
            var newFlowAccountKeys = new List<Flow.Entities.AccountKey> { flowAccountKey };

            var latestBlock = await flowClient.GetLatestBlockAsync();
            var emulatorAddress = new FlowAddress(emulatorAccount.Address);

            var transaction = AccountTransactions.CreateAccount(newFlowAccountKeys, emulatorAddress);

            transaction.Payer = emulatorAddress;
            transaction.ProposalKey = new FlowProposalKey
            {
                Address = emulatorAddress,
                KeyId = emulatorAccountKey.Index,
                SequenceNumber = emulatorAccountKey.SequenceNumber
            };
            transaction.ReferenceBlockId = latestBlock.Id;

            // sign and submit the transaction
            transaction = FlowTransaction.AddEnvelopeSignature(transaction, emulatorAddress, emulatorAccountKey.Index, emulatorAccountKey.Signer);

            var response = await flowClient.SendTransactionAsync(transaction);

            // wait for seal
            var sealedResponse = await flowClient.WaitForSealAsync(response);

            var createdEvent = sealedResponse.Events.FirstOrDefault(x => x.Type == "flow.AccountCreated");

            string addressHex = createdEvent.EventComposite.Data.FirstOrDefault().Value.Substring(2);

            var address = new FlowAddress(addressHex);

            var latestBlock2 = await flowClient.GetLatestBlockAsync();
            var account = await flowClient.GetAccountAsync(addressHex, latestBlock2.Height);

            account.Keys = FlowAccountKey.UpdateFlowAccountKeys(newFlowAccountKeys.Select(x => new FlowAccountKey(x)).ToList(), account.Keys.ToList());

            return account;
        }
    }
}