using System;
using Graffle.FlowSdk.Services.Models;
using Graffle.FlowSdk.Types;
using Graffle.FlowSdk.Services.RecursiveLengthPrefix;
using System.Collections.Generic;
using System.Linq;

namespace Graffle.FlowSdk.Services.Transactions
{
    public static class AccountTransactions
    {
        private static readonly string CreateAccountTemplate = @"
transaction(publicKeys: [String], contracts: { String: String})
{
	prepare(signer: AuthAccount)
	{
		let acct = AuthAccount(payer: signer)
		for key in publicKeys {
				acct.addPublicKey(key.decodeHex())
		}
		for contract in contracts.keys {
			acct.contracts.add(name: contract, code: contracts[contract]!.decodeHex())
		}
	}
}";

        public static FlowTransaction CreateAccount(
            IEnumerable<Flow.Entities.AccountKey> flowAccountKeys,
            FlowAddress authorizerAddress,
            IEnumerable<FlowContract> flowContracts = null)
        {
            if (flowAccountKeys == null || flowAccountKeys.Count() == 0)
                throw new Exception("Flow account key required.");

            var keysArray = new List<FlowValueType>();
            foreach (var key in flowAccountKeys)
            {
                keysArray.Add(
                    new StringType(
                        Rlp.EncodedAccountKey(key).ByteArrayToHex()
                    ));
            }
            var accountKeys = new ArrayType(keysArray);

            var contractsDictionary = new Dictionary<FlowValueType, FlowValueType>();
            if (flowContracts != null && flowContracts.Count() > 0)
            {
                foreach (var contract in flowContracts)
                {
                    contractsDictionary.Add(new StringType(contract.Name), new StringType(contract.Source.StringToHex()));
                }
            }
            var contracts = new DictionaryType(contractsDictionary);

            var tx = new FlowTransaction
            {
                Script = new FlowScript(CreateAccountTemplate)
            };

            // add arguments
            tx.Arguments =
                new List<FlowValueType>
                {
                    accountKeys,
                    contracts
                };

            // add authorizer
            tx.Authorizers.Add(authorizerAddress);

            return tx;
        }
    }
}