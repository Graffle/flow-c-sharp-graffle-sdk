using Graffle.FlowSdk.Services.Nodes;
using Graffle.FlowSdk.Types;
using Grpc.Net.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Graffle.FlowSdk.Services.Tests
{
    [TestClass]
    public class GraffleClientTests
    {
        public static IEnumerable<object[]> SPORKS() =>
         [
            [Sporks.GetSporkByName("MainNet")],
            [Sporks.GetSporkByName("TestNet")],
         ];

        [TestMethod]
        [DynamicData(nameof(SPORKS), DynamicDataSourceType.Method)]
        public async Task PingAsync(Spork spork)
        {
            var client = new GraffleClient(spork) { CadenceSerializer = CadenceSerializerVersion.Expando };
            Assert.IsTrue(await client.PingAsync());
        }

        [TestMethod]
        [DynamicData(nameof(SPORKS), DynamicDataSourceType.Method)]
        public async Task GetLatestBlockAsync(Spork spork)
        {
            var client = new GraffleClient(spork) { CadenceSerializer = CadenceSerializerVersion.Expando };
            var res = await client.GetLatestBlockAsync(true);

            Assert.IsNotNull(res);
            Assert.IsTrue(res.Height > 0);
        }

        [TestMethod]
        [DynamicData(nameof(SPORKS), DynamicDataSourceType.Method)]
        public async Task ExecuteScriptAtBlockHeightAsync(Spork spork)
        {
            string script = @"access(all) fun main(arg : String): String { return arg }";

            var bytes = Encoding.UTF8.GetBytes(script);

            var client = new GraffleClient(spork) { CadenceSerializer = CadenceSerializerVersion.Expando };
            var arg = new StringType("foo");
            var latestBlock = await client.GetLatestBlockAsync(true);

            var res = await client.ExecuteScriptAtBlockHeightAsync(latestBlock.Height, bytes, new List<FlowValueType>() { arg });
            var json = Encoding.UTF8.GetString(res.Value.ToByteArray());

            var opt = new JsonSerializerOptions();
            opt.Converters.Add(new FlowValueTypeConverter());

            var flowType = JsonSerializer.Deserialize<FlowValueType>(json, opt);
            Assert.IsNotNull(flowType);
            Assert.AreEqual("foo", ((StringType)flowType).Data);
        }

        [TestMethod]
        [DynamicData(nameof(SPORKS), DynamicDataSourceType.Method)]
        public async Task ExecuteScriptAtBlockHeightAsync_ComplexResult(Spork spork)
        {
            string script = "access(all) fun main(msg: String, ts: Int64, uuid: UInt64) : {String:AnyStruct} { return { \"msg\":msg, \"ts\":ts, \"uuid\":uuid }}";

            var bytes = Encoding.UTF8.GetBytes(script);

            var client = new GraffleClient(spork) { CadenceSerializer = CadenceSerializerVersion.Expando };
            var str = new StringType("arg1");
            var ts = new Int64Type(123L);
            var uuid = new UInt64Type(654ul);

            var latestBlock = await client.GetLatestBlockAsync(true);
            var res = await client.ExecuteScriptAtBlockHeightAsync(latestBlock.Height, bytes, new List<FlowValueType>() { str, ts, uuid });
            var json = Encoding.UTF8.GetString(res.Value.ToByteArray());

            var opt = new JsonSerializerOptions();
            opt.Converters.Add(new FlowValueTypeConverter());

            var flowType = JsonSerializer.Deserialize<FlowValueType>(json, opt);
            Assert.IsInstanceOfType(flowType, typeof(DictionaryType));

            var dict = flowType as DictionaryType;
            var keys = dict.Data.Keys.Cast<StringType>();

            var msgKey = keys.First(x => x.Data == "msg");
            Assert.AreEqual("String", msgKey.Type);
            Assert.AreEqual("msg", Cast<StringType>(msgKey).Data);

            var msgValue = dict.Data[msgKey];
            Assert.AreEqual("String", msgValue.Type);
            Assert.AreEqual(str.Data, Cast<StringType>(msgValue).Data);

            var tsKey = keys.First(x => x.Data == "ts");
            Assert.AreEqual("String", tsKey.Type);
            Assert.AreEqual("ts", Cast<StringType>(tsKey).Data);

            var tsValue = dict.Data[tsKey];
            Assert.AreEqual("Int64", tsValue.Type);
            Assert.AreEqual(ts.Data, Cast<Int64Type>(tsValue).Data);

            var uuidKey = keys.First(x => x.Data == "uuid");
            Assert.AreEqual("String", uuidKey.Type);
            Assert.AreEqual("uuid", Cast<StringType>(uuidKey).Data);

            var uuidValue = dict.Data[uuidKey];
            Assert.AreEqual("UInt64", uuidValue.Type);
            Assert.AreEqual(uuid.Data, Cast<UInt64Type>(uuidValue).Data);
        }

        [TestMethod]
        [DynamicData(nameof(SPORKS), DynamicDataSourceType.Method)]
        public async Task ExecuteScriptLatestBlockAsync_ComplexResult(Spork spork)
        {
            string script = "access(all) fun main(msg: String, ts: Int64, uuid: UInt64) : {String:AnyStruct} { return { \"msg\":msg, \"ts\":ts, \"uuid\":uuid }}";

            var bytes = Encoding.UTF8.GetBytes(script);

            var client = new GraffleClient(spork) { CadenceSerializer = CadenceSerializerVersion.Expando };
            var str = new StringType("arg1");
            var ts = new Int64Type(123L);
            var uuid = new UInt64Type(654ul);

            var res = await client.ExecuteScriptAtLatestBlockAsync(bytes, new List<FlowValueType>() { str, ts, uuid });
            var json = Encoding.UTF8.GetString(res.Value.ToByteArray());

            var opt = new JsonSerializerOptions();
            opt.Converters.Add(new FlowValueTypeConverter());

            var flowType = JsonSerializer.Deserialize<FlowValueType>(json, opt);
            Assert.IsInstanceOfType(flowType, typeof(DictionaryType));

            var dict = flowType as DictionaryType;
            var keys = dict.Data.Keys.Cast<StringType>();

            var msgKey = keys.First(x => x.Data == "msg");
            Assert.AreEqual("String", msgKey.Type);
            Assert.AreEqual("msg", Cast<StringType>(msgKey).Data);

            var msgValue = dict.Data[msgKey];
            Assert.AreEqual("String", msgValue.Type);
            Assert.AreEqual(str.Data, Cast<StringType>(msgValue).Data);

            var tsKey = keys.First(x => x.Data == "ts");
            Assert.AreEqual("String", tsKey.Type);
            Assert.AreEqual("ts", Cast<StringType>(tsKey).Data);

            var tsValue = dict.Data[tsKey];
            Assert.AreEqual("Int64", tsValue.Type);
            Assert.AreEqual(ts.Data, Cast<Int64Type>(tsValue).Data);

            var uuidKey = keys.First(x => x.Data == "uuid");
            Assert.AreEqual("String", uuidKey.Type);
            Assert.AreEqual("uuid", Cast<StringType>(uuidKey).Data);

            var uuidValue = dict.Data[uuidKey];
            Assert.AreEqual("UInt64", uuidValue.Type);
            Assert.AreEqual(uuid.Data, Cast<UInt64Type>(uuidValue).Data);
        }

        [TestMethod]
        public async Task GetEventsForHeightRangeAsync_MainNet()
        {
            using var rpc = GrpcChannel.ForAddress($"http://{Sporks.MainNet().Node}");
            var client = new GraffleClient(rpc, Sporks.MainNet()) { CadenceSerializer = CadenceSerializerVersion.Expando };

            var evs = await client.GetEventsForHeightRangeAsync("A.f919ee77447b7497.FlowFees.FeesDeducted", Sporks.MainNet().RootHeight, Sporks.MainNet().RootHeight + 249ul);
        }

        [TestMethod]
        public async Task GetEventsForHeightRangeAsync_TestNet()
        {
            using var rpc = GrpcChannel.ForAddress($"http://{Sporks.TestNet().Node}");
            var client = new GraffleClient(rpc, Sporks.TestNet()) { CadenceSerializer = CadenceSerializerVersion.Expando };

            var evs = await client.GetEventsForHeightRangeAsync("A.912d5440f7e3769e.FlowFees.FeesDeducted", Sporks.TestNet().RootHeight, Sporks.TestNet().RootHeight + 249ul);
        }

        private T Cast<T>(FlowValueType flowType) where T : FlowValueType
        {
            return (T)flowType;
        }
    }
}