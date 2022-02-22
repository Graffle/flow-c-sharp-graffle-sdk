using Graffle.FlowSdk.Types;
using System.Linq;

namespace Graffle.FlowSdk.Services
{
    public static class StructTypeExtensions
    {
        public static dynamic ConvertToObject(this StructType flowStruct)
        {
            return flowStruct.Fields.ToDictionary(f => f.Name.ToCamelCase(), f => ((dynamic)f.Value).Data);
        }
    }
}