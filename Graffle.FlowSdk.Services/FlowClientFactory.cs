using Graffle.FlowSdk.Services.Nodes;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Graffle.FlowSdk
{
    public sealed class FlowClientFactory : IFlowClientFactory
    {
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
        public IGraffleClient CreateFlowClient()
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
            return GenerateFlowClient(spork);
        }

        /// <summary>
        /// Creates a client with the specific spork
        /// </summary>
        /// <param name="spork"></param>
        /// <returns></returns>
        public IGraffleClient CreateFlowClient(Spork spork)
        {
            return GenerateFlowClient(spork);
        }

        /// <summary>
        /// creates a new spork from the name of the spork
        /// </summary>
        /// <param name="sporkName"></param>
        /// <returns></returns>
        public IGraffleClient CreateFlowClient(string sporkName)
        {
            var spork = Sporks.GetSporkByName(sporkName);
            return GenerateFlowClient(spork);
        }

        /// <summary>
        /// Searches to find the correct spork by the height of block
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        public IGraffleClient CreateFlowClient(ulong blockHeight)
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
            return GenerateFlowClient(spork);
        }

        /// <summary>
        /// Creates or finds an already available channel for the Spork
        /// </summary>
        /// <param name="spork"></param>
        /// <returns></returns>
        private IGraffleClient GenerateFlowClient(Spork spork)
        {
            IGraffleClient graffleClient;

            //check to see if a graffle client already exists for this spork in the cache
            if (!graffleClientCache.TryGetValue(spork.Name, out graffleClient))
            {
                //don't have a client for this spork in the cache
                //acquire lock so only one thread can insert into the cache
                var myLock = _locks.GetOrAdd("lock", x => new SemaphoreSlim(1, 1));
                myLock.Wait();

                try
                {
                    //we got the lock
                    //need to check the cache again because another thread may have inserted
                    if (!graffleClientCache.TryGetValue(spork.Name, out graffleClient))
                    {
                        //still not here let's add it
                        graffleClient = new GraffleClient(spork);
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