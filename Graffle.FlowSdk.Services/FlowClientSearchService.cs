using System;
using System.Threading.Tasks;
using Grpc.Core;

namespace Graffle.FlowSdk
{
    public class FlowClientSearchService
    {
        private readonly IFlowClientFactory flowClientFactory;
        public FlowClientSearchService(IFlowClientFactory flowClientFactory)
        {
            this.flowClientFactory = flowClientFactory;
        }

        public async Task<(ulong? start, ulong? end)> GetBlockHeightRangeByDateRangeAsync(DateTimeOffset start, DateTimeOffset? end = null)
        {
            ulong? startBlockHeight;
            ulong? endBlockHeight = null;

            //Create the initial main net client
            var flowClient = flowClientFactory.CreateFlowClient();
            //get the latest block to to start the search
            var latestBlockResponse = await flowClient.GetLatestBlockAsync();
            startBlockHeight = await BinarySearchChain(flowClient.FirstSpork.RootHeight, latestBlockResponse.Height, start, true);
            if (end.HasValue)
                endBlockHeight = await BinarySearchChain(flowClient.FirstSpork.RootHeight, latestBlockResponse.Height, end.Value, false);

            //check if that block is in the date range
            (ulong? start, ulong? end) result = (startBlockHeight, endBlockHeight);

            return result;
        }

        private async Task<ulong> BinarySearchChain(ulong startHeight, ulong endHeight, DateTimeOffset searchDate, bool leftSearch)
        {
            if (startHeight == endHeight)
                return startHeight;

            var midHeight = endHeight - ((endHeight - startHeight) / 2);

            //These 2 if checks stop from from landing on an odd 1 division and getting stuck in a loop. Basically searches side by side
            if (midHeight == startHeight)
            {
                return leftSearch ? startHeight : endHeight;
            }
            else if (midHeight == endHeight)
            {
                return leftSearch ? endHeight : startHeight;
            }

            //get the date of the midblock
            var flowClient = flowClientFactory.CreateFlowClient(midHeight);
            try
            {
                var searchBlockResponse = await flowClient.GetBlockByHeightAsync(midHeight);
                var searchBlockDate = searchBlockResponse.Timestamp;
                if (searchBlockDate.Date == searchDate.Date)
                {
                    //we need to check if the neighbors are also in the date range. If so then we need to keep searching
                    var neighborHeight = leftSearch ? midHeight - 1 : midHeight + 1;

                    var neighborSearchResult = await flowClient.GetBlockByHeightAsync(neighborHeight);
                    if (neighborSearchResult.Timestamp.Date == searchDate.Date)
                    {
                        //we need to keep searching to make sure we find the first iteration of this date.
                        startHeight = leftSearch ? startHeight : neighborHeight;
                        endHeight = leftSearch ? neighborHeight : endHeight;
                        return await BinarySearchChain(startHeight, endHeight, searchDate, leftSearch);
                    }

                    return midHeight;
                }
                else if (searchBlockDate.Date > searchDate.Date)
                {
                    return await BinarySearchChain(startHeight, midHeight, searchDate, leftSearch);
                }
                else if (searchBlockDate.Date < searchDate.Date)
                {
                    return await BinarySearchChain(midHeight, endHeight, searchDate, leftSearch);
                }

                return 0;
            }
            catch (Grpc.Core.RpcException grpcEx)
            {
                if (grpcEx.StatusCode == StatusCode.NotFound)
                {
                    startHeight += (ulong)(leftSearch ? -1 : 1);
                    startHeight = startHeight < flowClient.FirstSpork.RootHeight ? flowClient.FirstSpork.RootHeight : startHeight;
                    return await BinarySearchChain(startHeight, endHeight, searchDate, leftSearch);
                }


                throw;
            }
        }
    }
}