# flow-c-sharp-graffle-sdk

# Deserialize Cadence Json
```
var json = @"{""type"":""Address"",""value"":""0x66d6f450e25a4e22""}";

//returns IDictionary<string,object> for json objects, IList<object> for json array, or primitive type for json value
//object == "0x66d6f450e25a4e22"
var object = CadenceJsonInterpreter.ObjectFromCadenceJson(json);
```

# Create client:
```
    var flowClientFactory = new FlowClientFactory("MainNet" /*"TestNet"*/);
    var flowClient = flowClientFactory.CreateFlowClient();
```

# Get latest block
```
    var latestBlock = flowClient.GetLatestBlockAsync();
```

# Get latest sealed block
```
    var latestBlock = await flowClient.GetLatestBlockAsync(true);
```

# Search a block range for a specific event
```
    var eventId = "A.c1e4f4f4c4257510.Market.MomentPurchased";
    ulong startBlock = 22037959;
    ulong endBlock = 22037961;
    var eventsResponse = await flowClient.GetEventsForHeightRangeAsync(eventId, startBlock, endBlock);
```

# Execute script at block Id
```
    var latestBlockResponse = await this.flowClient.GetLatestBlockAsync(true);

    var helloWorldScript = @"
        pub fun main(): String {
            return ""Hello World""
        }               
    ";
    
    var scriptBytes = Encoding.ASCII.GetBytes(helloWorldScript);
    
    var scriptResponse = await flowClient.ExecuteScriptAtBlockIdAsync(latestBlockResponse.Id.HashToByteString(), scriptBytes, new List<FlowValueType>());
    var metaDataJson = Encoding.Default.GetString(scriptResponse.Value.ToByteArray());
    var result = StringType.FromJson(metaDataJson);
```


# Execute script at block height
```
    var latestBlockResponse = await this.flowClient.GetLatestBlockAsync(true);

    var helloWorldScript = @"
        pub fun main(): String {
            return ""Hello World""
        }               
    ";
    
    var scriptBytes = Encoding.ASCII.GetBytes(helloWorldScript);
    
    var scriptResponse = await flowClient.ExecuteScriptAtBlockIdAsync(latestBlockResponse.Height, scriptBytes, new List<FlowValueType>());
    var metaDataJson = Encoding.Default.GetString(scriptResponse.Value.ToByteArray());
    var result = StringType.FromJson(metaDataJson);
```

# Get block at height
```
    var blockResponse = await flowClient.GetBlockByHeightAsync(blockHeight);
```

# Get a transaction within a block
```
    var block = await flowClient.GetBlockByHeightAsync(blockHeight);
    var collectionId = block.CollectionGuarantees.FirstOrDefault().CollectionId;
    var collection = await flowClient.GetCollectionById(collectionId.HashToByteString());
    var transactionId = collection.TransactionIds.FirstOrDefault();
    var transactionResult = await flowClient.GetTransactionResult(transactionId.HashToByteString());
    var transaction = await flowClient.GetTransactionAsync(transactionId.HashToByteString());            
```

# Get account at block height
```
    var account = await flowClient.GetAccountAsync(addressHex, blockHeight);
```

# Get collection by Id
```
    var collection = await flowClient.GetCollectionById(collectionId.HashToByteString());
```

# Create new account
```
    //example given using emulator account, load real account from flow.json for working with MainNet or TestNet
    var emulatorAccount = await flowClient.GetAccountFromConfigAsync("emulator-account");

    var emulatorAccountKey = emulatorAccount.Keys[0];

    var flowAccountKey = Flow.Entities.AccountKey.GenerateRandomEcdsaKey(SignatureAlgorithm.ECDSA_P256, HashAlgorithm.SHA3_256);
    var newFlowAccountKeys = new List<Flow.Entities.AccountKey> { flowAccountKey };

    var latestBlock = await flowClient.GetLatestBlockAsync();
    var emulatorAddress = new FlowAddress(emulatorAccount.Address);

    var transaction = AccountTransactions.CreateAccount(newFlowAccountKeys, emulatorAddress);

    transaction.Payer = emulatorAddress;
    transaction.ProposalKey = new FlowProposalKey
    {
        Address = emulatorAddress,
        KeyId = emulatorAccountKey.Index,
        SequenceNumber = emulatorAccountKey.SequenceNumber
    };
    transaction.ReferenceBlockId = latestBlock.Id;

    // sign and submit the transaction
    transaction = FlowTransaction.AddEnvelopeSignature(transaction, emulatorAddress, emulatorAccountKey.Index, emulatorAccountKey.Signer);

    var response = await flowClient.SendTransactionAsync(transaction);

    // wait for seal
    var sealedResponse = await flowClient.WaitForSealAsync(response);

    var createdEvent = sealedResponse.Events.FirstOrDefault(x => x.Type == "flow.AccountCreated");

    string addressHex = createdEvent.EventComposite.Data.FirstOrDefault().Value.Substring(2);

    var address = new FlowAddress(addressHex);

    var latestBlock2 = await flowClient.GetLatestBlockAsync();
    var account = await flowClient.GetAccountAsync(addressHex, latestBlock2.Height);

    account.Keys = FlowAccountKey.UpdateFlowAccountKeys(newFlowAccountKeys.Select(x => new FlowAccountKey(x)).ToList(), account.Keys.ToList());
```

# Send transaction
```
    var emulatorAccount = await flowClient.GetAccountFromConfigAsync("emulator-account");
    var emulatorAddress = new FlowAddress(emulatorAccount.Address);
    var emulatorAccountKey = emulatorAccount.Keys[0];

    var latestBlock = await flowClient.GetLatestBlockAsync();
    var transaction = new FlowTransaction
    {
        Script = new FlowScript("transaction {}"),
        ReferenceBlockId = latestBlock.Id,
        Payer = emulatorAddress,
        ProposalKey = new FlowProposalKey
        {
            Address = emulatorAddress,
            KeyId = emulatorAccountKey.Index,
            SequenceNumber = emulatorAccountKey.SequenceNumber
        }
    };

    transaction = FlowTransaction.AddEnvelopeSignature(transaction, emulatorAddress, emulatorAccountKey.Index, emulatorAccountKey.Signer);

    var response = await flowClient.SendTransactionAsync(transaction);

    // wait for seal
    var sealedResponse = await flowClient.WaitForSealAsync(response);
```