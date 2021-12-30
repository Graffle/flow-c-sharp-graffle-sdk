using Google.Protobuf;
using Graffle.FlowSdk;
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

        public string Address { get; set; }
        public ulong Balance { get; set; }
        public string Code { get; set; }
        public IEnumerable<FlowAccountKey> Keys { get; set; }
        public Dictionary<string, string>  Contracts { get; set; }
    }
}