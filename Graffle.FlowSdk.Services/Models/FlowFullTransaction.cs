using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flow.Entities;
using Google.Protobuf;
using Graffle.FlowSdk.Types;

namespace Graffle.FlowSdk.Services.Models
{
    public class FlowFullTransaction
    {
        public FlowFullTransaction(IFlowTransactionResult flowTransactionResult, IFlowTransaction flowTransaction)
        {
            FlowTransactionResult = flowTransactionResult;
            FlowTransaction = flowTransaction;
        }

        public IFlowTransactionResult FlowTransactionResult { get; }
        public IFlowTransaction FlowTransaction { get; }
    }
}