using System;
using Graffle.FlowSdk.Services.Nodes;
using Grpc.Core;
using Grpc.Net.Client;
using System.Runtime.Caching;

namespace Graffle.FlowSdk.Services
{
    public class GraffleClientFactory : IGraffleClientFactory
    {
        private static readonly GrpcChannelOptions GRPC_CHANNEL_OPTIONS = new GrpcChannelOptions()
        {
            Credentials = ChannelCredentials.Insecure,
            MaxReceiveMessageSize = null, //null = no limit
        };

        private readonly NodeType _nodeType;
        public GraffleClientFactory(NodeType nodeType)
        {
            _nodeType = nodeType;
        }

        public GraffleClientFactory(string nodeType)
        {
            _nodeType = nodeType.ToLower() switch
            {
                "mainnet" => NodeType.MainNet,
                "testnet" => NodeType.TestNet,
                "emulator" => NodeType.Emulator,
                _ => throw new ArgumentException($"Invalid flow node {nodeType}")
            };
        }

        public IGraffleClient Create()
        {
            var spork = CurrentSpork();
            var channel = GetRpcChannelInternal(spork.Node);
            return new GraffleClient(channel, spork);
        }

        public IGraffleClient Create(ulong blockHeight)
        {
            var spork = GetSpork(blockHeight);
            var channel = GetRpcChannelInternal(spork.Node);
            return new GraffleClient(channel, spork);
        }

        public IGraffleClient Create(string accessNodeUri)
        {
            var currentSpork = CurrentSpork();
            var spork = new Spork(currentSpork.Name, accessNodeUri, currentSpork.RootHeight, null, _nodeType == NodeType.TestNet, _nodeType == NodeType.Emulator);
            var channel = GetRpcChannelInternal(accessNodeUri);
            return new GraffleClient(channel, spork);
        }

        private GrpcChannel GetRpcChannelInternal(string uri)
        {
            string fullUri; //get a new reference for the lazy
            if (!uri.StartsWith("http://"))
                fullUri = $"http://{uri}";
            else
                fullUri = uri;

            //wrap rpc channel creation in lazy for concurrent access
            var lazy = new Lazy<GrpcChannel>(() => GrpcChannel.ForAddress(fullUri, GRPC_CHANNEL_OPTIONS));
            var cachePolicy = new CacheItemPolicy()
            {
                RemovedCallback = ItemRemovedCallback,
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(60)
            };

            var result = MemoryCache.Default.AddOrGetExisting(uri, lazy, cachePolicy) as Lazy<GrpcChannel>;
            return result.Value;
        }

        private Spork CurrentSpork()
        {
            return _nodeType switch
            {
                NodeType.MainNet => Sporks.MainNet(),
                NodeType.TestNet => Sporks.TestNet(),
                NodeType.Emulator => Sporks.GetEmulatorSporkByHeight(1ul),
                _ => throw new InvalidOperationException($"Invalid flow node {_nodeType}")
            };
        }

        private Spork GetSpork(ulong blockHeight)
        {
            return _nodeType switch
            {
                NodeType.MainNet => Sporks.GetMainSporkByHeight(blockHeight),
                NodeType.TestNet => Sporks.GetDevSporkByHeight(blockHeight),
                NodeType.Emulator => Sporks.GetEmulatorSporkByHeight(blockHeight),
                _ => throw new InvalidOperationException($"Invalid flow node {_nodeType}")
            };
        }

        private static void ItemRemovedCallback(CacheEntryRemovedArguments args)
        {
            if (args.CacheItem is IDisposable disposable)
            {
                try
                {
                    disposable?.Dispose();
                }
                catch (Exception) { }
            }
        }
    }
}