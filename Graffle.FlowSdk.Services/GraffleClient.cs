using Google.Protobuf;
using Graffle.FlowSdk.Services;
using Graffle.FlowSdk.Services.Models;
using Graffle.FlowSdk.Services.Nodes;
using Graffle.FlowSdk.Types;
using Grpc.Core;
using Grpc.Net.Client;
using Polly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Graffle.FlowSdk
{
    public sealed class GraffleClient : IGraffleClient
    {
        public Spork CurrentSpork { get; private set; }
        public Spork FirstSpork
        {
            get
            {
                if (CurrentSpork == null)
                    return null;

                return CurrentSpork.IsTestNet ? Sporks.GetSporkByName(TestNetSporks.TestNet17.Name) : (!CurrentSpork.IsEmulator ? Sporks.GetSporkByName(MainNetSporks.MainNet1.Name) : Sporks.GetSporkByName(EmulatorSporks.Emulator.Name));
            }
        }

        public Spork LatestSpork
        {
            get
            {
                if (CurrentSpork == null)
                    return null;

                return CurrentSpork.IsTestNet ? Sporks.GetSporkByName(TestNetSporks.TestNet.Name) : (!CurrentSpork.IsEmulator ? Sporks.GetSporkByName(MainNetSporks.MainNet.Name) : Sporks.GetSporkByName(EmulatorSporks.Emulator.Name));
            }
        }

        private FlowClient flowClient { get; }

        public GraffleClient(Spork spork)
        {
            this.flowClient = FlowClient.Create(spork.Node);
            this.CurrentSpork = spork;
        }

        public GraffleClient(GrpcChannel rpcChannel, Spork spork)
        {
            this.flowClient = FlowClient.Create(rpcChannel);
            this.CurrentSpork = spork;
        }

        public async Task<bool> PingAsync()
        {
            return await flowClient.Ping();
        }

        public async Task<FlowBlock> GetLatestBlockAsync(bool isSealed = true, CallOptions options = new CallOptions())
        {
            return new FlowBlock((await flowClient.GetLatestBlockAsync(isSealed, options)).Block);
        }

        public async Task<List<FlowEvent>> GetEventsForHeightRangeAsync(string eventType, ulong startHeight, ulong endHeight, CallOptions options = new CallOptions())
        {
            return FlowEvent.Create((await flowClient.GetEventsForHeightRangeAsync(eventType, startHeight, endHeight, options)).Results);
        }

        public async Task<Flow.Access.ExecuteScriptResponse> ExecuteScriptAtBlockHeightAsync(ulong blockHeight, byte[] cadenceScript, IEnumerable<FlowValueType> args, CallOptions options = new CallOptions())
        {
            return await flowClient.ExecuteScriptAtBlockHeightAsync(blockHeight, cadenceScript, args, options);
        }

        public async Task<Flow.Access.ExecuteScriptResponse> ExecuteScriptAtBlockIdAsync(ByteString blockId, byte[] cadenceScript, IEnumerable<FlowValueType> args, CallOptions options = new CallOptions())
        {
            return await flowClient.ExecuteScriptAtBlockIdAsync(blockId, cadenceScript, args, options);
        }

        public async Task<FlowBlock> GetBlockByHeightAsync(ulong blockHeight, CallOptions options = new CallOptions())
        {
            return new FlowBlock((await flowClient.GetBlockByHeightAsync(blockHeight, options)).Block);
        }

        public async Task<FlowTransaction> GetTransactionAsync(ByteString transactionId)
        {
            return new FlowTransaction(await flowClient.GetTransactionAsync(transactionId), false);
        }

        public async Task<FlowAccount> GetAccountAsync(string address, ulong blockHeight)
        {
            return new FlowAccount(await flowClient.GetAccountAsync(address, blockHeight));
        }

        public async Task<FlowCollection> GetCollectionById(ByteString collectionId)
        {
            return new FlowCollection((await flowClient.GetCollectionById(collectionId)).Collection);
        }

        public async Task<FlowTransactionResult> GetTransactionResult(ByteString transactionId)
        {
            return new FlowTransactionResult(await flowClient.GetTransactionResult(transactionId));
        }

        public async Task<FlowTransactionResponse> SendTransactionAsync(FlowTransaction flowTransaction, CallOptions options = new CallOptions())
        {
            try
            {
                var transaction = flowTransaction.FromFlowTransaction();

                var response = await flowClient.SendTransactionAsync(
                    new Flow.Access.SendTransactionRequest
                    {
                        Transaction = transaction
                    }, options);

                return response.ToFlowSendTransactionResponse();
            }
            catch (Exception exception)
            {
                throw new Exception("SendTransaction request failed.", exception);
            }
        }

        public async Task<FlowTransactionResult> WaitForSealAsync(FlowTransactionResponse transactionResponse, int delayMs = 1000, int timeoutMS = 30000)
        {
            var startTime = DateTime.UtcNow;
            while (true)
            {
                var result = await GetTransactionResult(transactionResponse.Id);

                if (result != null && result.Status == Flow.Entities.TransactionStatus.Sealed)
                    return result;

                if (DateTime.UtcNow.Subtract(startTime).TotalMilliseconds > timeoutMS)
                    throw new Exception("Timed out waiting for seal.");

                await Task.Delay(delayMs);
            }
        }

        public async Task<Flow.Entities.Account> GetAccountFromConfigAsync(string name, string filePath = null)
        {
            return await flowClient.GetAccountFromConfigAsync(name, filePath);
        }

        public async Task<FlowFullTransaction> GetCompleteTransactionAsync(ByteString transactionId)
        {
            var retryPolicy = Policy
                            .Handle<Exception>()
                            .WaitAndRetryAsync(new[]
                            {
                                TimeSpan.FromMilliseconds(250),
                                TimeSpan.FromMilliseconds(500),
                                TimeSpan.FromMilliseconds(2000)
                            });

            var transaction = await retryPolicy.ExecuteAsync(() => GetTransactionAsync(transactionId));

            FlowTransactionResult transactionResult = null;
            try
            {
                transactionResult = await retryPolicy.ExecuteAsync(() => GetTransactionResult(transactionId));
            }
            catch { } //eat the exception, might just be a script with no events/result

            var result = new FlowFullTransaction(transactionResult, transaction);

            return result;
        }
    }
}