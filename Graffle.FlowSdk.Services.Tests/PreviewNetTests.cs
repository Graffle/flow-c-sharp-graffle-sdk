using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Graffle.FlowSdk.Services.Nodes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graffle.FlowSdk.Services.Tests
{
    [TestClass]
    public class PreviewNetTests
    {
        [TestMethod]
        public async Task PreviewNet_Test()
        {
            using var http = new HttpClient();
            using var channel = GrpcChannel.ForAddress($"http://{Sporks.PreviewNet().Node}", new GrpcChannelOptions()
            {
                MaxReceiveMessageSize = null,
                Credentials = ChannelCredentials.Insecure,
                HttpClient = http
            });

            var client = new GraffleClient(channel, Sporks.PreviewNet());
            await client.PingAsync();
        }
    }
}