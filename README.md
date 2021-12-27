# flow-c-sharp-graffle-sdk

# Create client:
```
    var flowClientFactory = new FlowClientFactory(nodeName);
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