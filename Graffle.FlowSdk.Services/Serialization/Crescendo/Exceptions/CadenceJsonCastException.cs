using System;
using System.Diagnostics;

namespace Graffle.FlowSdk.Services.Serialization
{
    [DebuggerDisplay("Expected Type {ExpectedType} Actual Type {ActualType}")]
    public class CadenceJsonCastException(string message) : Exception(message)
    {
        public Type ExpectedType { get; init; }
        public Type ActualType { get; init; }

        override public string Message
        {
            get
            {
                return string.Format("{0}, Expected Type {1}, Actual Type {2}", base.Message, ExpectedType?.ToString() ?? "null", ActualType?.ToString() ?? "null");
            }
        }
    }
}