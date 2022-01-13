using System;
using System.Collections.Generic;
using System.Text.Json;
using Flow.Entities;
using Newtonsoft.Json;

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

        [JsonConstructor]
        public FlowTransactionResult(string blockId, string errorMessage, TransactionStatus status, string statusDescription, uint statusCode, IList<FlowTransactionResponseEvent> events)
        {
            BlockId = blockId;
            ErrorMessage = errorMessage;
            Status = status;
            StatusDescription = statusDescription;
            StatusCode = statusCode;
            Events = events;
        }

        [JsonProperty("blockId")]
        public string BlockId { get; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; }

        [JsonProperty("status")]
        public TransactionStatus Status { get; }

        [JsonProperty("statusDescription")]
        public string StatusDescription { get; }

        [JsonProperty("statusCode")]
        public uint StatusCode { get; }

        [JsonProperty("events")]
        public IList<FlowTransactionResponseEvent> Events { get; } = new List<FlowTransactionResponseEvent>();
    }
}