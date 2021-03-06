using Graffle.FlowSdk.Services.Models;
using Nethereum.RLP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Graffle.FlowSdk.Services.RecursiveLengthPrefix
{
    public class Rlp
    {
        public static byte[] EncodedAccountKey(Flow.Entities.AccountKey flowAccountKey)
        {
            var accountElements = new List<byte[]>
            {
                RLP.EncodeElement(flowAccountKey.PublicKey.ByteStringToHex().HexToBytes()),
                RLP.EncodeElement(ConvertorForRLPEncodingExtensions.ToBytesFromNumber(BitConverter.GetBytes((uint)flowAccountKey.SignatureAlgorithm))),
                RLP.EncodeElement(ConvertorForRLPEncodingExtensions.ToBytesFromNumber(BitConverter.GetBytes((uint)flowAccountKey.HashAlgorithm))),
                RLP.EncodeElement(ConvertorForRLPEncodingExtensions.ToBytesFromNumber(BitConverter.GetBytes(flowAccountKey.Weight)))
            };

            return RLP.EncodeList(accountElements.ToArray());
        }

        public static byte[] EncodedCanonicalPayload(FlowTransaction flowTransaction)
        {
            var payloadElements = new List<byte[]>
            {
                RLP.EncodeElement(flowTransaction.Script.RawScript.ToBytesForRLPEncoding()),
                RLP.EncodeList(flowTransaction.Arguments.Select(argument => RLP.EncodeElement(argument.AsJsonCadenceDataFormat().ToBytesForRLPEncoding())).ToArray()),
                RLP.EncodeElement(Helpers.Pad(flowTransaction.ReferenceBlockId.HexToByteString().ToByteArray(), 32)),
                RLP.EncodeElement(ConvertorForRLPEncodingExtensions.ToBytesFromNumber(BitConverter.GetBytes(flowTransaction.GasLimit))),
                RLP.EncodeElement(Helpers.Pad(flowTransaction.ProposalKey.Address.Value.ToByteArray(), 8)),
                RLP.EncodeElement(ConvertorForRLPEncodingExtensions.ToBytesFromNumber(BitConverter.GetBytes(flowTransaction.ProposalKey.KeyId))),
                RLP.EncodeElement(ConvertorForRLPEncodingExtensions.ToBytesFromNumber(BitConverter.GetBytes(flowTransaction.ProposalKey.SequenceNumber))),
                RLP.EncodeElement(Helpers.Pad(flowTransaction.Payer.Value.ToByteArray(), 8)),
                RLP.EncodeList(flowTransaction.Authorizers.Select(authorizer => RLP.EncodeElement(Helpers.Pad(authorizer.Value.ToByteArray(), 8))).ToArray())
            };

            return RLP.EncodeList(payloadElements.ToArray());
        }

        public static byte[] EncodedSignatures(FlowSignature[] signatures, FlowTransaction flowTransaction)
        {
            var signatureElements = new List<byte[]>();
            for (var i = 0; i < signatures.Length; i++)
            {
                var index = i;
                if (flowTransaction.SignerList.ContainsKey(signatures[i].Address.Value.ToHash()))
                {
                    index = flowTransaction.SignerList[signatures[i].Address.Value.ToHash()];
                }
                else
                {
                    flowTransaction.SignerList.Add(signatures[i].Address.Value.ToHash(), i);
                }

                var signatureEncoded = EncodedSignature(signatures[i], index);
                signatureElements.Add(signatureEncoded);
            }

            return RLP.EncodeList(signatureElements.ToArray());
        }        

        public static byte[] EncodedSignature(FlowSignature signature, int index)
        {
            var signatureArray = new List<byte[]>
            {
                RLP.EncodeElement(index.ToBytesForRLPEncoding()),
                RLP.EncodeElement(ConvertorForRLPEncodingExtensions.ToBytesFromNumber(BitConverter.GetBytes(signature.KeyId))),
                RLP.EncodeElement(signature.Signature)
            };

            return RLP.EncodeList(signatureArray.ToArray());
        }

        public static byte[] EncodedCanonicalAuthorizationEnvelope(FlowTransaction flowTransaction)
        {
            var authEnvelopeElements = new List<byte[]>
            {
                EncodedCanonicalPayload(flowTransaction),
                EncodedSignatures(flowTransaction.PayloadSignatures.ToArray(), flowTransaction)
            };

            return RLP.EncodeList(authEnvelopeElements.ToArray());
        }

        public static byte[] EncodedCanonicalPaymentEnvelope(FlowTransaction flowTransaction)
        {
            var authEnvelopeElements = new List<byte[]>
            {
                EncodedCanonicalAuthorizationEnvelope(flowTransaction),
                EncodedSignatures(flowTransaction.EnvelopeSignatures.ToArray(), flowTransaction)
            };

            return RLP.EncodeList(authEnvelopeElements.ToArray());
        }

        public static byte[] EncodedCanonicalTransaction(FlowTransaction flowTransaction)
        {
            var authEnvelopeElements = new List<byte[]>
            {
                EncodedCanonicalPayload(flowTransaction),
                EncodedSignatures(flowTransaction.PayloadSignatures.ToArray(), flowTransaction),
                EncodedSignatures(flowTransaction.EnvelopeSignatures.ToArray(), flowTransaction)
            };

            return RLP.EncodeList(authEnvelopeElements.ToArray());
        }
    }
}
