using Graffle.FlowSdk.Services.RecursiveLengthPrefix;
using Graffle.FlowSdk.Cryptography;
using System.Collections.Generic;
using Google.Protobuf;
using Graffle.FlowSdk.Types;
using System.Linq;

namespace Graffle.FlowSdk.Services.Models
{
    public interface IFlowTransaction
    {
        FlowScript Script { get; set; }
        IList<FlowValueType> Arguments { get; set; }
        string ReferenceBlockId { get; set; }
        ulong GasLimit { get; set; }
        FlowAddress Payer { get; set; }
        FlowProposalKey ProposalKey { get; set; }
        IList<FlowAddress> Authorizers { get; set; }
        IList<FlowSignature> PayloadSignatures { get; set; }
        IList<FlowSignature> EnvelopeSignatures { get; set; }
        Dictionary<string, int> SignerList { get; set; }
    }

    public sealed class FlowTransaction : IFlowTransaction
    {
        public FlowTransaction(Flow.Access.TransactionResponse transaction)
        {
            Script = new FlowScript(transaction.Transaction.Script.ToString(System.Text.Encoding.UTF8));

            //TODO: Make sure this works. Missing Args
            var thing = transaction.Transaction.Arguments.Select(s => s.ToString(System.Text.Encoding.UTF8));

            ReferenceBlockId = transaction.Transaction.ReferenceBlockId.ToHash();
            GasLimit = transaction.Transaction.GasLimit;
            Payer = new FlowAddress(transaction.Transaction.Payer);
            ProposalKey = new FlowProposalKey(transaction.Transaction.ProposalKey);
            Authorizers = transaction.Transaction.Authorizers.Select(s => new FlowAddress(s)).ToList();
            PayloadSignatures = transaction.Transaction.PayloadSignatures.Select(s => new FlowSignature(s)).ToList();
            EnvelopeSignatures = transaction.Transaction.EnvelopeSignatures.Select(s => new FlowSignature(s)).ToList();
            var signatureList = new List<FlowSignature>();
            signatureList.AddRange(PayloadSignatures);
            signatureList.AddRange(EnvelopeSignatures);
            foreach (var item in signatureList)
            {
                SignerList.TryAdd(item.Address.Value.ToHash(), signatureList.IndexOf(item));
            }
        }
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
        public string ReferenceBlockId { get; set; }
        public ulong GasLimit { get; set; }
        public FlowAddress Payer { get; set; }
        public FlowProposalKey ProposalKey { get; set; }
        public IList<FlowAddress> Authorizers { get; set; }
        public IList<FlowSignature> PayloadSignatures { get; set; }
        public IList<FlowSignature> EnvelopeSignatures { get; set; }
        public Dictionary<string, int> SignerList { get; set; } = new Dictionary<string, int>();

        public static FlowTransaction AddPayloadSignature(FlowTransaction flowTransaction, FlowAddress address, uint keyId, IMessageSigner signer)
        {
            var canonicalPayload = Rlp.EncodedCanonicalPayload(flowTransaction);
            var message = DomainTag.AddTransactionDomainTag(canonicalPayload);
            var signature = signer.Sign(message);

            flowTransaction.PayloadSignatures.Add(
                new FlowSignature
                {
                    Address = new FlowAddress(address.Value),
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
                    Address = new FlowAddress(address.Value),
                    KeyId = keyId,
                    Signature = signature
                });

            return flowTransaction;
        }
    }
}