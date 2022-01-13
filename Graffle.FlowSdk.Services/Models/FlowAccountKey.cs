using Google.Protobuf;
using Graffle.FlowSdk;
using System.Collections.Generic;
using Graffle.FlowSdk.Cryptography;
using Google.Protobuf.Collections;
using System.Linq;
using Newtonsoft.Json;

namespace Graffle.FlowSdk.Services.Models {
    public class FlowAccountKey
    {
        public FlowAccountKey(Flow.Entities.AccountKey accountKey) {
            Index = accountKey.Index;
            PublicKey = accountKey.PublicKey.ToHash();
            PrivateKey = accountKey.PrivateKey;
            SignatureAlgorithm = (SignatureAlgorithm)accountKey.SignatureAlgorithm;
            HashAlgorithm = (HashAlgorithm)accountKey.HashAlgorithm;
            Weight = accountKey.Weight;
            SequenceNumber = accountKey.SequenceNumber;
            Revoked = accountKey.Revoked;
        }

        [JsonConstructor]
        public FlowAccountKey(uint index, string publicKey, string privateKey, SignatureAlgorithm signatureAlgorithm, HashAlgorithm hashAlgorithm, uint weight, uint sequenceNumber, bool revoked, IMessageSigner signer)
        {
            Index = index;
            PublicKey = publicKey;
            PrivateKey = privateKey;
            SignatureAlgorithm = signatureAlgorithm;
            HashAlgorithm = hashAlgorithm;
            Weight = weight;
            SequenceNumber = sequenceNumber;
            Revoked = revoked;
            Signer = signer;
        }

        [JsonProperty("index")]
        public uint Index { get; set; }

        [JsonProperty("publicKey")]
        public string PublicKey { get; set; }

        [JsonProperty("privateKey")]
        public string PrivateKey { get; set; }

        [JsonProperty("signatureAlgorithm")]
        public SignatureAlgorithm SignatureAlgorithm { get; set; }

        [JsonProperty("hashAlgorithm")]
        public HashAlgorithm HashAlgorithm { get; set; }

        [JsonProperty("weight")]
        public uint Weight { get; set; }

        [JsonProperty("sequenceNumber")]
        public uint SequenceNumber { get; set; }

        [JsonProperty("revoked")]
        public bool Revoked { get; set; }

        [JsonProperty("signer")]
        public IMessageSigner Signer { get; set; }

        public static IList<FlowAccountKey> UpdateFlowAccountKeys(IList<FlowAccountKey> currentFlowAccountKeys, IList<FlowAccountKey> updatedFlowAccountKeys)
        {
            foreach(var key in updatedFlowAccountKeys)
            {
                var currentKey = currentFlowAccountKeys.Where(w => w.PublicKey == key.PublicKey).FirstOrDefault();
                if(currentKey != null && !string.IsNullOrEmpty(currentKey.PrivateKey))
                {
                    key.PrivateKey = currentKey.PrivateKey;
                    key.Signer = ECDSA.CreateSigner(key.PrivateKey, key.SignatureAlgorithm, key.HashAlgorithm);
                }
            }

            return updatedFlowAccountKeys;
        }
    }
}