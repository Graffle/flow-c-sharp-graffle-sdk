using Graffle.FlowSdk.Services.RecursiveLengthPrefix;
using Graffle.FlowSdk.Cryptography;
using System.Collections.Generic;
using Google.Protobuf;
using Graffle.FlowSdk.Types;

namespace Graffle.FlowSdk.Services.Models
{
    public sealed class FlowTransaction
    {
        public FlowTransaction()
        {
            Arguments = new List<FlowValueType>();
            Authorizers = new List<FlowAddress>();
            PayloadSignatures = new List<FlowSignature>();
            EnvelopeSignatures = new List<FlowSignature>();
            GasLimit = 9999;
        }

        public FlowScript Script { get; set; }
        public IList<FlowValueType> Arguments { get; set; }
        public ByteString ReferenceBlockId { get; set; }
        public ulong GasLimit { get; set; }
        public FlowAddress Payer { get; set; }
        public FlowProposalKey ProposalKey { get; set; }
        public IList<FlowAddress> Authorizers { get; set; }
        public IList<FlowSignature> PayloadSignatures { get; set; }
        public IList<FlowSignature> EnvelopeSignatures { get; set; }
        public Dictionary<ByteString, int> SignerList { get; set; }

        public static FlowTransaction AddPayloadSignature(FlowTransaction flowTransaction, FlowAddress address, uint keyId, IMessageSigner signer)
        {
            var canonicalPayload = Rlp.EncodedCanonicalPayload(flowTransaction);
            var message = DomainTag.AddTransactionDomainTag(canonicalPayload);
            var signature = signer.Sign(message);

            flowTransaction.PayloadSignatures.Add(
                new FlowSignature
                {
                    Address = address.Value,
                    KeyId = keyId,
                    Signature = signature
                });

            return flowTransaction;
        }

        public static FlowTransaction AddEnvelopeSignature(FlowTransaction flowTransaction, FlowAddress address, uint keyId, IMessageSigner signer)
        {
            var canonicalAuthorizationEnvelope = Rlp.EncodedCanonicalAuthorizationEnvelope(flowTransaction);
            var message = DomainTag.AddTransactionDomainTag(canonicalAuthorizationEnvelope);
            var signature = signer.Sign(message);

            flowTransaction.EnvelopeSignatures.Add(
                new FlowSignature
                {
                    Address = address.Value,
                    KeyId = keyId,
                    Signature = signature
                });

            return flowTransaction;
        }
    }
}