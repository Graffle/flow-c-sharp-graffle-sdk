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
using Graffle.FlowSdk.Services.RecursiveLengthPrefix;

namespace Graffle.FlowSdk.Services.Tests.AccountTests
{
    [TestClass]
    [Ignore]
    public class CreateAccountTests
    {
        private IGraffleClient flowClient { get; }

        public CreateAccountTests(){
            var factory = new FlowClientFactory(NodeType.Emulator);
            this.flowClient = factory.CreateFlowClient();
        }

        [TestMethod]
        public void GenerateRandomEcdsaKey_Creates_Key_Successfully()
        {
            uint weight = 500;
            var signatureAlgo = SignatureAlgorithm.ECDSA_P256;
            var hashAlgo = HashAlgorithm.SHA3_256;
            var accountKey = Flow.Entities.AccountKey.GenerateRandomEcdsaKey(signatureAlgo, hashAlgo, weight);

            Assert.AreEqual(signatureAlgo, accountKey.SignatureAlgorithm);
            Assert.AreEqual(hashAlgo, accountKey.HashAlgorithm);
            Assert.AreEqual(weight, accountKey.Weight);
            Assert.AreEqual((ulong)0, accountKey.SequenceNumber);
            Assert.AreEqual((uint)0, accountKey.Index);
        }

        [TestMethod]
        public async Task Given_Valid_Parameters_When_Creating_An_Account_Then_Account_Is_Created_Successfully(){
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
            
            Assert.AreEqual(Flow.Entities.TransactionStatus.Sealed, sealedResponse.Status, "Test failed waiting for emulator block to seal.");
            Assert.AreEqual(7, sealedResponse.Events.Count);

            var sealedTransaction = await flowClient.GetTransactionAsync(response.Id);
            Assert.AreEqual(2, sealedTransaction.Arguments.Count);
            Assert.AreEqual("Array", sealedTransaction.Arguments[0].Type);
            Assert.AreEqual(1, (sealedTransaction.Arguments[0] as ArrayType).Data.Count);
            Assert.AreEqual(Rlp.EncodedAccountKey(newFlowAccountKeys[0]).ByteArrayToHex(), ((sealedTransaction.Arguments[0] as ArrayType).Data[0] as StringType).Data);
            Assert.AreEqual("Dictionary", sealedTransaction.Arguments[1].Type);
            Assert.AreEqual(0, (sealedTransaction.Arguments[1] as DictionaryType).Data.Count);
            Assert.AreEqual(emulatorAddress.HexValue, sealedTransaction.Payer.HexValue);
            Assert.AreEqual(emulatorAddress.HexValue, sealedTransaction.Authorizers[0].HexValue);            
        }
    }
}