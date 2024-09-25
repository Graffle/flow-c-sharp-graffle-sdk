using Graffle.FlowSdk.Services;
using Graffle.FlowSdk.Services.Nodes;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Graffle.FlowSdk
{
    public sealed class FlowClientFactory : IFlowClientFactory
    {
        /// <summary>
        /// If true CadenceSerializerVersion.Crescendo will be used for all sporks
        /// Otherwise the CadenceSerializerVersion used will be based on the individual spork requested
        /// </summary>
        public bool UseCrescendoSerializerForAllSporks { get; init; } = true;

        private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();
        private readonly MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                                                                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));

        private readonly MemoryCache graffleClientCache = new(new MemoryCacheOptions());

        private readonly NodeType _nodeType;

        public FlowClientFactory(NodeType nodeType)
        {
            _nodeType = nodeType;
        }

        public FlowClientFactory(string nodeType)
        {
            _nodeType = nodeType.ToLower() switch
            {
                "mainnet" => NodeType.MainNet,
                "testnet" => NodeType.TestNet,
                "emulator" => NodeType.Emulator,
                _ => throw new NotSupportedException($"Node Type: {nodeType} is not yet supported."),
            };
        }

        /// <summary>
        /// This creates a client from the latest Spork
        /// </summary>
        /// <returns></returns>
        public IGraffleClient CreateFlowClient(bool cacheOverride = false)
        {
            var spork = _nodeType switch
            {
                NodeType.MainNet => Sporks.GetSporkByName(MainNetSporks.MainNet.Name),
                NodeType.TestNet => Sporks.GetSporkByName(TestNetSporks.TestNet.Name),
                NodeType.Emulator => Sporks.GetSporkByName(EmulatorSporks.Emulator.Name),
                _ => throw new NotSupportedException($"Node Type: {_nodeType} is not yet supported."),
            };
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
            ArgumentException.ThrowIfNullOrEmpty(accessNodeUri);

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
            var spork = _nodeType switch
            {
                NodeType.MainNet => Sporks.GetMainSporkByHeight(blockHeight),
                NodeType.TestNet => Sporks.GetDevSporkByHeight(blockHeight),
                NodeType.Emulator => Sporks.GetEmulatorSporkByHeight(blockHeight),
                _ => throw new NotSupportedException($"Node Type: {_nodeType} is not yet supported."),
            };
            return GenerateFlowClientInternal(spork, cacheOverride);
        }

        /// <summary>
        /// Creates or finds an already available channel for the Spork
        /// </summary>
        /// <param name="spork"></param>
        /// <returns></returns>
        private IGraffleClient GenerateFlowClientInternal(Spork spork, bool cacheOverride)
        {
            //check to see if a graffle client already exists for this spork in the cache
            if (cacheOverride || !graffleClientCache.TryGetValue(spork.Name, out IGraffleClient graffleClient))
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
                        var serializerVersion = UseCrescendoSerializerForAllSporks || Sporks.IsCrescendo(spork) ?
                            CadenceSerializerVersion.Crescendo : CadenceSerializerVersion.Legacy;

                        graffleClient = new GraffleClient(spork) { CadenceSerializer = serializerVersion };
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