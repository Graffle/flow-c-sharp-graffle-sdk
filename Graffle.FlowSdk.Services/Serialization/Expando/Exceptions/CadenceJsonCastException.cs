using System;

namespace Graffle.FlowSdk.Services.Serialization
{
    public class CadenceJsonCastException(string message) : Exception(message)
    {
        public Type ExpectedType { get; init; }
        public Type ActualType { get; init; }

        override public string ToString() =>
            string.Format("{0}, Expected Type {1}, Actual Type {2}", base.Message, ExpectedType?.ToString() ?? "null", ActualType?.ToString() ?? "null");
    }
}