using Google.Protobuf;
using Graffle.FlowSdk;
using System.Collections.Generic;
using Graffle.FlowSdk.Cryptography;
using Google.Protobuf.Collections;

namespace Graffle.FlowSdk.Services.Models {
    public class FlowAccountKey
    {
        public FlowAccountKey(Flow.Entities.AccountKey accountKey) {
            Index = accountKey.Index;
            PublicKey = accountKey.PublicKey.ToHash();
            SignatureAlgorithm = (SignatureAlgorithm)accountKey.SignatureAlgorithm;
            HashAlgorithm = (HashAlgorithm)accountKey.HashAlgorithm;
            Weight = accountKey.Weight;
            SequenceNumber = accountKey.SequenceNumber;
            Revoked = accountKey.Revoked;
        }
        
        public uint Index { get; set; }
        public string PublicKey { get; set; }
        public SignatureAlgorithm SignatureAlgorithm { get; set;}
        public HashAlgorithm HashAlgorithm { get; set; }
        public uint Weight { get; set; }
        public uint SequenceNumber { get; set; }
        public bool Revoked { get; set; }
    }
}