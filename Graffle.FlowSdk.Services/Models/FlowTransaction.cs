using Graffle.FlowSdk.Services.RecursiveLengthPrefix;
using Graffle.FlowSdk.Cryptography;
using System.Collections.Generic;
using Google.Protobuf;
using Graffle.FlowSdk.Types;
using System.Linq;
using System.Text.Json;
using System;
using Newtonsoft.Json;

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
        private JsonSerializerOptions options;

        public FlowTransaction(Flow.Access.TransactionResponse transaction, bool includeArguments = true)
        {
            Script = new FlowScript(transaction.Transaction.Script.ToString(System.Text.Encoding.UTF8));

            this.options = new JsonSerializerOptions();
            this.options.Converters.Add(new FlowCompositeTypeConverter());
            this.options.Converters.Add(new GraffleCompositeTypeConverter());
            this.options.Converters.Add(new FlowValueTypeConverter());

            if (transaction.Transaction.Arguments != null && transaction.Transaction.Arguments.Any() && includeArguments)
            {
                Arguments = transaction.Transaction.Arguments.Select(s =>
                                FlowValueType.CreateFromCadence(s.ToString(System.Text.Encoding.UTF8)))
                                .ToList();
            }

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

        [JsonConstructor]
        public FlowTransaction(FlowScript script, IList<FlowValueType> arguments, string referenceBlockId, ulong gasLimit, FlowAddress payer, FlowProposalKey proposalKey, IList<FlowAddress> authorizers, IList<FlowSignature> payloadSignatures, IList<FlowSignature> envelopeSignatures, Dictionary<string, int> signerList)
        {
            Script = script;
            Arguments = arguments;
            ReferenceBlockId = referenceBlockId;
            GasLimit = gasLimit;
            Payer = payer;
            ProposalKey = proposalKey;
            Authorizers = authorizers;
            PayloadSignatures = payloadSignatures;
            EnvelopeSignatures = envelopeSignatures;
            SignerList = signerList;
        }

        [JsonProperty("script")]
        public FlowScript Script { get; set; }

        //TODO: this is being ignored due to some issues with comples arguments to Flowtype. Will Fix Later
        [JsonIgnore]
        [JsonProperty("arguments")]
        public IList<FlowValueType> Arguments { get; set; }

        [JsonProperty("referenceBlockId")]
        public string ReferenceBlockId { get; set; }

        [JsonProperty("gasLimit")]
        public ulong GasLimit { get; set; }

        [JsonProperty("payer")]
        public FlowAddress Payer { get; set; }

        [JsonProperty("proposalKey")]
        public FlowProposalKey ProposalKey { get; set; }

        [JsonProperty("authorizers")]
        public IList<FlowAddress> Authorizers { get; set; }

        [JsonProperty("payloadSignatures")]
        public IList<FlowSignature> PayloadSignatures { get; set; }

        [JsonProperty("envelopeSignatures")]
        public IList<FlowSignature> EnvelopeSignatures { get; set; }

        [JsonProperty("signerList")]
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