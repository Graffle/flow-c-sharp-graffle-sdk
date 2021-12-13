using System;
using System.Collections.Concurrent;
using Graffle.FlowSdk.Services.Nodes;

namespace Graffle.FlowSdk
{
    public sealed class FlowClientFactory
    {
        private ConcurrentDictionary<string, GraffleClient> ChannelCache = new ConcurrentDictionary<string, GraffleClient>();
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
        public GraffleClient CreateFlowClient()
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
        public GraffleClient CreateFlowClient(Spork spork)
        {
            return GenerateFlowClient(spork);
        }

        /// <summary>
        /// creates a new spork from the name of the spork
        /// </summary>
        /// <param name="sporkName"></param>
        /// <returns></returns>
        public GraffleClient CreateFlowClient(string sporkName)
        {
            var spork = Sporks.GetSporkByName(sporkName);
            return GenerateFlowClient(spork);
        }

        /// <summary>
        /// Searches to find the correct spork by the height of block
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        public GraffleClient CreateFlowClient(ulong blockHeight)
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
        private GraffleClient GenerateFlowClient(Spork spork)
        {
            //Check to see if the channel already exists
            var graffleClientFound = ChannelCache.TryGetValue(spork.Name, out var graffleClient);
            if (!graffleClientFound)
            {
                graffleClient = new GraffleClient(spork);
                ChannelCache.TryAdd(spork.Name, graffleClient);
            }

            return graffleClient;
        }

        /// <summary>
        /// Clears the channel cache
        /// </summary>
        public void ResetChannelCache()
        {
            ChannelCache.Clear();
        }
    }
}