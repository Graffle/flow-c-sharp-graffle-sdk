using Graffle.FlowSdk.Services.Nodes;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Graffle.FlowSdk
{
    public sealed class FlowClientFactory : IFlowClientFactory
    {
        public bool UseBetaDeserializer { get; init; } = false;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new ConcurrentDictionary<string, SemaphoreSlim>();
        private readonly MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                                                                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));

        private readonly MemoryCache graffleClientCache = new MemoryCache(new MemoryCacheOptions());

        private readonly NodeType nodeType;

        public FlowClientFactory(NodeType nodeType)
        {
            this.nodeType = nodeType;
        }

        public FlowClientFactory(string nodeType)
        {
            switch (nodeType.ToLower())
            {
                case "mainnet":
                    this.nodeType = NodeType.MainNet;
                    break;
                case "testnet":
                    this.nodeType = NodeType.TestNet;
                    break;
                case "emulator":
                    this.nodeType = NodeType.Emulator;
                    break;
                default:
                    throw new NotSupportedException($"Node Type: {nodeType} is not yet supported.");
            }
        }

        /// <summary>
        /// This creates a client from the latest Spork
        /// </summary>
        /// <returns></returns>
        public IGraffleClient CreateFlowClient(bool cacheOverride = false)
        {
            Spork spork = null;
            switch (nodeType)
            {
                case NodeType.MainNet:
                    spork = Sporks.GetSporkByName(MainNetSporks.MainNet.Name);
                    break;
                case NodeType.TestNet:
                    spork = Sporks.GetSporkByName(TestNetSporks.TestNet.Name);
                    break;
                case NodeType.Emulator:
                    spork = Sporks.GetSporkByName(EmulatorSporks.Emulator.Name);
                    break;
                default:
                    throw new NotSupportedException($"Node Type: {nodeType} is not yet supported.");
            }
            return GenerateFlowClientInternal(spork, cacheOverride);
        }

        /// <summary>
        /// Creates a client with the specific spork
        /// </summary>
        /// <param name="spork"></param>
        /// <returns></returns>
        public IGraffleClient CreateFlowClient(Spork spork, bool cacheOverride = false)
        {
            return GenerateFlowClientInternal(spork, cacheOverride);
        }

        public IGraffleClient CreateFlowClientFromUri(string accessNodeUri, bool cacheOverride = false)
        {
            if (string.IsNullOrEmpty(accessNodeUri))
                throw new ArgumentException();

            //kind of a hack here, create a temporary spork with the given uri
            //start and end height not needed
            return GenerateFlowClientInternal(new Spork(accessNodeUri, accessNodeUri, 0, 0, false, false), cacheOverride);
        }

        /// <summary>
        /// creates a new spork from the name of the spork
        /// </summary>
        /// <param name="sporkName"></param>
        /// <returns></returns>
        public IGraffleClient CreateFlowClient(string sporkName, bool cacheOverride = false)
        {
            var spork = Sporks.GetSporkByName(sporkName);
            return GenerateFlowClientInternal(spork, cacheOverride);
        }

        /// <summary>
        /// Searches to find the correct spork by the height of block
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        public IGraffleClient CreateFlowClient(ulong blockHeight, bool cacheOverride = false)
        {
            Spork spork = null;
            switch (nodeType)
            {
                case NodeType.MainNet:
                    spork = Sporks.GetMainSporkByHeight(blockHeight);
                    break;
                case NodeType.TestNet:
                    spork = Sporks.GetDevSporkByHeight(blockHeight);
                    break;
                case NodeType.Emulator:
                    spork = Sporks.GetEmulatorSporkByHeight(blockHeight);
                    break;
                default:
                    throw new NotSupportedException($"Node Type: {nodeType} is not yet supported.");
            }
            return GenerateFlowClientInternal(spork, cacheOverride);
        }

        /// <summary>
        /// Creates or finds an already available channel for the Spork
        /// </summary>
        /// <param name="spork"></param>
        /// <returns></returns>
        private IGraffleClient GenerateFlowClientInternal(Spork spork, bool cacheOverride)
        {
            IGraffleClient graffleClient;

            //check to see if a graffle client already exists for this spork in the cache
            if (cacheOverride || !graffleClientCache.TryGetValue(spork.Name, out graffleClient))
            {
                //don't have a client for this spork in the cache
                //acquire lock so only one thread can insert into the cache
                var myLock = _locks.GetOrAdd(spork.Name, x => new SemaphoreSlim(1, 1));
                myLock.Wait();

                try
                {
                    //we got the lock
                    //need to check the cache again because another thread may have inserted
                    if (cacheOverride || !graffleClientCache.TryGetValue(spork.Name, out graffleClient))
                    {
                        //still not here let's add it
                        graffleClient = new GraffleClient(spork) { UseBetaDeserializer = UseBetaDeserializer };
                        graffleClientCache.Set(spork.Name, graffleClient, cacheEntryOptions);
                    }
                }
                finally
                {
                    myLock.Release();
                }
            }

            return graffleClient;
        }

        private bool _isDisposed = false;

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    graffleClientCache?.Dispose();

                    foreach (var l in _locks)
                        l.Value?.Dispose();
                    _locks?.Clear();
                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}