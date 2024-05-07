using System;
using Graffle.FlowSdk.Services;
using Graffle.FlowSdk.Services.Nodes;

namespace Graffle.FlowSdk
{
    public interface IFlowClientFactory : IDisposable
    {
        public CadenceSerializerVersion CandeceSerializer { get; }

        /// <summary>
        /// This creates a client from the latest Spork
        /// </summary>
        /// <returns></returns>
        IGraffleClient CreateFlowClient(bool cacheOverride = false);

        /// <summary>
        /// Creates a client with the specific spork
        /// </summary>
        /// <param name="spork"></param>
        /// <returns></returns>
        IGraffleClient CreateFlowClient(Spork spork, bool cacheOverride = false);

        /// <summary>
        /// creates a new spork from the name of the spork
        /// </summary>
        /// <param name="sporkName"></param>
        /// <returns></returns>
        IGraffleClient CreateFlowClient(string sporkName, bool cacheOverride = false);

        /// <summary>
        /// Searches to find the correct spork by the height of block
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        IGraffleClient CreateFlowClient(ulong blockHeight, bool cacheOverride = false);

        IGraffleClient CreateFlowClientFromUri(string accessNodeUri, bool cacheOverride = false);
    }
}