using System.Collections.Generic;
using System.Threading.Tasks;
using Graffle.FlowSdk.Services.Nodes;
using Graffle.FlowSdk.Types;
using Google.Protobuf;
using Grpc.Core;
using Graffle.FlowSdk.Services.Models;

namespace Graffle.FlowSdk
{
    public interface IGraffleClient {
        Spork CurrentSpork { get; }
        
        Spork FirstSpork { get; }
        
        Spork LatestSpork { get; }
        
        Task PingAsync();

        Task<FlowBlock> GetLatestBlockAsync(bool isSealed = true, CallOptions options = new CallOptions());

        Task<List<FlowEvent>> GetEventsForHeightRangeAsync(string eventType, ulong startHeight, ulong endHeight, CallOptions options = new CallOptions());

        Task<Flow.Access.ExecuteScriptResponse> ExecuteScriptAtBlockHeightAsync(ulong blockHeight, byte[] cadenceScript, IEnumerable<FlowValueType> args, CallOptions options = new CallOptions());

        Task<Flow.Access.ExecuteScriptResponse> ExecuteScriptAtBlockIdAsync(ByteString blockId, byte[] cadenceScript, IEnumerable<FlowValueType> args, CallOptions options = new CallOptions());

        Task<FlowBlock> GetBlockByHeightAsync(ulong blockHeight, CallOptions options = new CallOptions());

        Task<FlowTransaction> GetTransactionAsync(ByteString transactionId);

        Task<FlowAccount> GetAccountAsync(string address, ulong blockHeight);

        Task<FlowCollection> GetCollectionById(ByteString collectionId);

        Task<FlowTransactionResult> GetTransactionResult(ByteString transactionId);
        Task<FlowFullTransaction> GetCompleteTransactionAsync(ByteString transactionId);

        Task<FlowTransactionResponse> SendTransactionAsync(FlowTransaction flowTransaction, CallOptions options = new CallOptions());
        
        Task<FlowTransactionResult> WaitForSealAsync(FlowTransactionResponse transactionResponse, int delayMs = 1000, int timeoutMS = 30000);

        Task<Flow.Entities.Account> GetAccountFromConfigAsync(string name, string filePath = null);
    }
}