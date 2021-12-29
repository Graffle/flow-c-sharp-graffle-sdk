using System;
using System.Collections.Generic;
using System.Text.Json;
using Flow.Entities;

namespace Graffle.FlowSdk.Services.Models
{
    public interface IFlowTransactionResult
    {
        string BlockId { get; }
        string ErrorMessage { get; }
        TransactionStatus Status { get; }
        string StatusDescription { get; }
        uint StatusCode { get; }
        IList<FlowTransactionResponseEvent> Events { get; }
    }
    public class FlowTransactionResult : IFlowTransactionResult
    {
        public FlowTransactionResult(Flow.Access.TransactionResultResponse flowTransactionResponse)
        {
            BlockId = flowTransactionResponse.BlockId.ToHash();
            ErrorMessage = flowTransactionResponse.ErrorMessage;
            Status = flowTransactionResponse.Status;
            StatusDescription = Enum.GetName(typeof(TransactionStatus), flowTransactionResponse.Status);
            StatusCode = flowTransactionResponse.StatusCode;

            var options = new JsonSerializerOptions();
            options.Converters.Add(new FlowCompositeTypeConverter());
            options.Converters.Add(new GraffleCompositeTypeConverter());
            options.Converters.Add(new FlowValueTypeConverter());

            foreach (var item in flowTransactionResponse.Events)
            {
                Events.Add(new FlowTransactionResponseEvent(item, flowTransactionResponse.BlockId, options));
            }
        }

        public string BlockId { get; }
        public string ErrorMessage { get; }
        public TransactionStatus Status { get; }
        public string StatusDescription { get; }
        public uint StatusCode { get; }
        public IList<FlowTransactionResponseEvent> Events { get; } = new List<FlowTransactionResponseEvent>();
    }
}