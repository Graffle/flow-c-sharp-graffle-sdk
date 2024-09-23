using Graffle.FlowSdk.Services.Nodes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Graffle.FlowSdk.Services.Tests.SerializationTests;

[TestClass]
[Ignore]
public class SerializerVersionTests
{
    [TestMethod]
    public async Task GetEvents_Legacy()
    {
        using var channel = GrpcChannel.ForAddress($"http://{Sporks.MainNet().Node}", new GrpcChannelOptions()
        {
            Credentials = ChannelCredentials.Insecure,
            MaxReceiveMessageSize = null,
        });

        var client = new GraffleClient(channel, Sporks.MainNet());
        var evs = await client.GetEventsForHeightRangeAsync("A.d4a8f8d167745a51.Heartbeat.heartbeat", 78618043, 78618043);

        Assert.IsTrue(evs.Count > 0);
        Assert.AreEqual(CadenceSerializerVersion.Legacy, evs.First().EventComposite.SerializerVersion);
    }

    [TestMethod]
    [Ignore]
    public async Task GetEvents_Expando()
    {
        using var channel = GrpcChannel.ForAddress($"http://{Sporks.MainNet().Node}", new GrpcChannelOptions()
        {
            Credentials = ChannelCredentials.Insecure,
            MaxReceiveMessageSize = null,
        });

        var client = new GraffleClient(channel, Sporks.MainNet()) { CadenceSerializer = CadenceSerializerVersion.Crescendo };
        var evs = await client.GetEventsForHeightRangeAsync("A.d4a8f8d167745a51.Heartbeat.heartbeat", 78618043, 78618043);
        Assert.IsTrue(evs.Count > 0);
        Assert.AreEqual(CadenceSerializerVersion.Crescendo, evs.First().EventComposite.SerializerVersion);
    }

    [TestMethod]
    [Ignore]
    public async Task GetTransaction_Legacy()
    {
        using var channel = GrpcChannel.ForAddress($"http://{Sporks.MainNet().Node}", new GrpcChannelOptions()
        {
            Credentials = ChannelCredentials.Insecure,
            MaxReceiveMessageSize = null,
        });

        var client = new GraffleClient(channel, Sporks.MainNet());
        var txnId = "9b94faa1e37670931d1841beab237b5fa5b32f786b38e55187231d1fc40faf29".HashToByteString();
        var txn = await client.GetTransactionResult(txnId);

        Assert.IsNotNull(txn);
        Assert.AreEqual(CadenceSerializerVersion.Legacy, txn.Events.First().EventComposite.SerializerVersion);
    }

    [TestMethod]
    [Ignore]
    public async Task GetTransaction_Expando()
    {
        using var channel = GrpcChannel.ForAddress($"http://{Sporks.MainNet().Node}", new GrpcChannelOptions()
        {
            Credentials = ChannelCredentials.Insecure,
            MaxReceiveMessageSize = null,
        });

        var client = new GraffleClient(channel, Sporks.MainNet()) { CadenceSerializer = CadenceSerializerVersion.Crescendo };
        var txnId = "9b94faa1e37670931d1841beab237b5fa5b32f786b38e55187231d1fc40faf29".HashToByteString();
        var txn = await client.GetTransactionResult(txnId);

        Assert.IsNotNull(txn);
        Assert.AreEqual(CadenceSerializerVersion.Crescendo, txn.Events.First().EventComposite.SerializerVersion);
    }
}
