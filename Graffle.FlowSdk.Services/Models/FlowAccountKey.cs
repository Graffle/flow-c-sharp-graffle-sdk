using Google.Protobuf;
using Graffle.FlowSdk;
using System.Collections.Generic;
using Graffle.FlowSdk.Cryptography;
using Google.Protobuf.Collections;
using System.Linq;

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

        public uint Index { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public SignatureAlgorithm SignatureAlgorithm { get; set; }
        public HashAlgorithm HashAlgorithm { get; set; }
        public uint Weight { get; set; }
        public uint SequenceNumber { get; set; }
        public bool Revoked { get; set; }
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