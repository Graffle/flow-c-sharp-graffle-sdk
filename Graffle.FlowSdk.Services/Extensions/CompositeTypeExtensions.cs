using Graffle.FlowSdk.Services.Extensions;
using Graffle.FlowSdk.Types;
using System.Collections.Generic;
using System.Linq;

namespace Graffle.FlowSdk.Services
{
    public static class CompositeTypeExtensions
    {
        /// <summary>
        /// Return struct data as primitive types (string, int, etc) ie not objects of type FlowValueType
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Dictionary<string, dynamic> FieldsAsDictionary(this CompositeType x)
        {
            return x.Fields.ToDictionary(f => f.Name,
                    f =>
                    {
                        var value = f.Value;
                        return FlowValueTypeUtility.FlowTypeToPrimitive(value);
                    });
        }
    }
}