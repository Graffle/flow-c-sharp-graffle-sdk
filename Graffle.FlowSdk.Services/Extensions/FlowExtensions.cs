using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Graffle.FlowSdk.Services.Models;
using Graffle.FlowSdk.Types;

namespace Graffle.FlowSdk.Services {
    public static class FlowExtensions {
        public static Flow.Entities.Transaction FromFlowTransaction(this FlowTransaction flowTransaction)
        {
            var tx = new Flow.Entities.Transaction
            {
                Script = flowTransaction.Script.RawScript.StringToByteString(),
                Payer = flowTransaction.Payer.Value,
                GasLimit = flowTransaction.GasLimit,
                ReferenceBlockId = flowTransaction.ReferenceBlockId,
                ProposalKey = flowTransaction.ProposalKey.FromFlowProposalKey()
            };

            if (flowTransaction.Arguments != null && flowTransaction.Arguments.Any())
            {
                foreach (var argument in flowTransaction.Arguments){
                    tx.Arguments.Add(argument.DataAsJson().StringToByteString());
                }
            }

            foreach(var authorizer in flowTransaction.Authorizers)
                tx.Authorizers.Add(authorizer.Value);

            foreach(var payloadSignature in flowTransaction.PayloadSignatures)
                tx.PayloadSignatures.Add(payloadSignature.FromFlowSignature());

            foreach (var envelopeSignature in flowTransaction.EnvelopeSignatures)
                tx.EnvelopeSignatures.Add(envelopeSignature.FromFlowSignature());

            return tx;
        }

        public static Flow.Entities.Transaction.Types.ProposalKey FromFlowProposalKey(this FlowProposalKey flowProposalKey)
        {
            return new Flow.Entities.Transaction.Types.ProposalKey
            {
                Address = flowProposalKey.Address.Value,
                KeyId = flowProposalKey.KeyId,
                SequenceNumber = flowProposalKey.SequenceNumber
            };
        }

        public static Flow.Entities.Transaction.Types.Signature FromFlowSignature(this FlowSignature flowSignature)
        {
            return new Flow.Entities.Transaction.Types.Signature
            {
                Address = flowSignature.Address,
                KeyId = flowSignature.KeyId,
                Signature_ = flowSignature.Signature.ByteArrayToByteString()
            };
        }

        public static FlowTransactionResponse ToFlowSendTransactionResponse(this Flow.Access.SendTransactionResponse sendTransactionResponse)
        {
            return new FlowTransactionResponse
            {
                Id = sendTransactionResponse.Id
            };
        }
    }
}