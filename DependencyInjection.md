# gRPC Client Dependency Injection

Inject gRPC for latest spork

```
var spork = Sporks.MainNet(); //Sporks.TestNet()
services.AddGrpcClient<AccessAPIClient>((_, o) =>
    {
        o.Address = new Uri($"http://{spork.Node}");
    })
.ConfigureChannel(o =>
    {
        o.MaxReceiveMessageSize = null;
        o.Credentials = ChannelCredentials.Insecure;
        o.UnsafeUseInsecureChannelCallCredentials = true;
    });
services.AddTransient(typeof(IGraffleClient), x =>
   {
       var rpcClient = x.GetRequiredService<AccessAPIClient>();
       var flowClient = new FlowClient(rpcClient);
       return new GraffleClient(flowClient, spork);
   });
```

# Old Sporks

Manually create gRPC client

```
var spork = Sporks.GetSporkByName("MainNet24");
var options = new GrpcChannelOptions()
{
    Credentials = ChannelCredentials.Insecure,
    MaxReceiveMessageSize = null, //null = no limit
};
using var channel = GrpcChannel.ForAddress($"http://{spork.Node}", options);
var client = new GraffleClient(channel, spork);
await client.PingAsync();
```