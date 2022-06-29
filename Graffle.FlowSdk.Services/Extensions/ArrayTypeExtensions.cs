using Graffle.FlowSdk.Types;
using System.Collections.Generic;

namespace Graffle.FlowSdk.Services.Extensions
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Return array data as primitive types (string, int, etc) ie not objects of type FlowValueType
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static List<dynamic> ToValueData(this ArrayType x)
        {
            var result = new List<dynamic>();
            foreach (var item in x.Data)
            {
                result.Add(FlowValueTypeUtility.FlowTypeToPrimitive(item));
            }
            return result;
        }
    }
}