using Google.Protobuf;
using Graffle.FlowSdk;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Graffle.FlowSdk.Services.Models {
    public class FlowAccount
    {
        public FlowAccount(Flow.Access.AccountResponse accountResponse) {
            Address = accountResponse.Account.Address.ToHash();
            Balance = accountResponse.Account.Balance;
            Code = accountResponse.Account.Code.ToHash();
            Keys = accountResponse.Account.Keys.Select(x => new FlowAccountKey(x));
            Contracts = accountResponse.Account.Contracts.ToDictionary(x => x.Key, x => x.Value.ToHash());
        }

        [JsonConstructor]
        public FlowAccount(string address, ulong balance, string code, IEnumerable<FlowAccountKey> keys, Dictionary<string, string> contracts)
        {
            Address = address;
            Balance = balance;
            Code = code;
            Keys = keys;
            Contracts = contracts;
        }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("balance")]
        public ulong Balance { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("keys")]
        public IEnumerable<FlowAccountKey> Keys { get; set; }

        [JsonProperty("contracts")]
        public Dictionary<string, string>  Contracts { get; set; }
    }
}