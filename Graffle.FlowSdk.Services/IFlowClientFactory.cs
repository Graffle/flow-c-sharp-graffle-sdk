using Graffle.FlowSdk.Services.Nodes;

namespace Graffle.FlowSdk
{
    public interface IFlowClientFactory
    {
        /// <summary>
        /// This creates a client from the latest Spork
        /// </summary>
        /// <returns></returns>
        IGraffleClient CreateFlowClient();

        /// <summary>
        /// Creates a client with the specific spork
        /// </summary>
        /// <param name="spork"></param>
        /// <returns></returns>
        IGraffleClient CreateFlowClient(Spork spork);

        /// <summary>
        /// creates a new spork from the name of the spork
        /// </summary>
        /// <param name="sporkName"></param>
        /// <returns></returns>
        IGraffleClient CreateFlowClient(string sporkName);

        /// <summary>
        /// Searches to find the correct spork by the height of block
        /// </summary>
        /// <param name="blockHeight"></param>
        /// <returns></returns>
        IGraffleClient CreateFlowClient(ulong blockHeight);
    }
}