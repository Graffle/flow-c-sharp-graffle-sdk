using System.Threading.Tasks;
using Graffle.FlowSdk.Services.Models;
using Graffle.FlowSdk.Services.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Graffle.FlowSdk.Services.RecursiveLengthPrefix;

namespace Graffle.FlowSdk.Services.Tests.TransactionsTests
{
    [TestClass]
    public class TransactionEncodingTests
    {
        private IGraffleClient flowClient { get; }

        public TransactionEncodingTests(){
            var factory = new FlowClientFactory(NodeType.Emulator);
            this.flowClient = factory.CreateFlowClient();
        }

        [TestMethod]
        public void Test_Complete_Transaction_With_Envelope_Signature() {
            const string payload = "f872b07472616e73616374696f6e207b2065786563757465207b206c6f67282248656c6c6f2c20576f726c64212229207d207dc0a0f0e4c2f76c58916ec258f246851bea091d14d4247a2fc3e18694461b1816e13b2a880000000000000001040a880000000000000001c9880000000000000001";
            const string envelope = "f899f872b07472616e73616374696f6e207b2065786563757465207b206c6f67282248656c6c6f2c20576f726c64212229207d207dc0a0f0e4c2f76c58916ec258f246851bea091d14d4247a2fc3e18694461b1816e13b2a880000000000000001040a880000000000000001c9880000000000000001e4e38004a0f7225388c1d69d57e6251c9fda50cbbf9e05131e5adb81e5aa0422402f048162";

            var transaction = Transaction();
            transaction.EnvelopeSignatures.Add(
                new FlowSignature
                {
                    Address = new FlowAddress("01".HexToByteString()),
                    KeyId = 4,
                    Signature = "f7225388c1d69d57e6251c9fda50cbbf9e05131e5adb81e5aa0422402f048162".HexToBytes()
                });

            var payloadTest = Rlp.EncodedCanonicalPayload(transaction);
            Assert.AreEqual(payload, payloadTest.ByteArrayToHex());

            var envelopeTest = Rlp.EncodedCanonicalAuthorizationEnvelope(transaction);
            Assert.AreEqual(envelope, envelopeTest.ByteArrayToHex());
        }

        private static FlowTransaction Transaction()
        {
            var transaction = new FlowTransaction
            {
                ProposalKey = new FlowProposalKey
                {
                    Address = new FlowAddress("01"),
                    KeyId = 4,
                    SequenceNumber = 10
                },
                GasLimit = 42,
                Payer = new FlowAddress("01"),
                ReferenceBlockId = "f0e4c2f76c58916ec258f246851bea091d14d4247a2fc3e18694461b1816e13b",
                Script = new FlowScript("transaction { execute { log(\"Hello, World!\") } }")
            };

            transaction.Authorizers.Add(new FlowAddress("01"));

            transaction.PayloadSignatures.Add(
                new FlowSignature
                {
                    Address = new FlowAddress("01".HexToByteString()),
                    KeyId = 4,
                    Signature = "f7225388c1d69d57e6251c9fda50cbbf9e05131e5adb81e5aa0422402f048162".HexToBytes()
                });

            return transaction;
        }
    }
}