using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Graffle.FlowSdk.Services.Nodes;
using Graffle.FlowSdk.Services.Serialization;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graffle.FlowSdk.Services.Tests
{
    [TestClass]
    public class PreviewNetTests
    {
        private static HttpClient _http;

        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            _http = new HttpClient();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _http?.Dispose();
        }

        [TestMethod]
        public async Task PreviewNet_Test()
        {
            using var channel = GrpcChannel.ForAddress($"http://{Sporks.PreviewNet().Node}", new GrpcChannelOptions()
            {
                MaxReceiveMessageSize = null,
                Credentials = ChannelCredentials.Insecure,
                HttpClient = _http
            });

            var client = new GraffleClient(channel, Sporks.PreviewNet());
            await client.PingAsync();
        }

        [TestMethod]
        public async Task Test()
        {
            using var channel = GrpcChannel.ForAddress($"http://{Sporks.PreviewNet().Node}", new GrpcChannelOptions()
            {
                MaxReceiveMessageSize = null,
                Credentials = ChannelCredentials.Insecure,
                HttpClient = _http
            });

            var client = new GraffleClient(channel, Sporks.PreviewNet());

            var evs = await client.GetEventsForHeightRangeAsync("evm.BlockExecuted", 8056207, 8056207);

            var x = evs.FirstOrDefault()?.EventComposite;
        }

        [TestMethod]
        public void x()
        {
            var json = "{\"value\":{\"id\":\"evm.BlockExecuted\",\"fields\":[{\"value\":{\"value\":\"3247\",\"type\":\"UInt64\"},\"name\":\"height\"},{\"value\":{\"value\":\"0x678d4e6d9a6449d63e31224715751ba10a54c624f0ff438bbd9acc1d051d083c\",\"type\":\"String\"},\"name\":\"hash\"},{\"value\":{\"value\":\"126632307892400000000000\",\"type\":\"Int\"},\"name\":\"totalSupply\"},{\"value\":{\"value\":\"0x6172e7d655e71e035bd5f666cba1e87b920c3a3d87c3e3440611bfe48d583e99\",\"type\":\"String\"},\"name\":\"parentHash\"},{\"value\":{\"value\":\"0x6e8d187dcac1dee8b0ec6e34cb24d426388ef82c91910618c8fc4b664609ead1\",\"type\":\"String\"},\"name\":\"receiptRoot\"},{\"value\":{\"value\":[{\"value\":\"0x81b62a4e8cf828430d579cee359cc80da9db2122ca1e0c15047ed7d63d766baa\",\"type\":\"String\"}],\"type\":\"Array\"},\"name\":\"transactionHashes\"}]},\"type\":\"Event\"}";

            var y = JsonCadenceInterchangeFormatDeserializer.FromEventPayload(json);
        }
    }
}