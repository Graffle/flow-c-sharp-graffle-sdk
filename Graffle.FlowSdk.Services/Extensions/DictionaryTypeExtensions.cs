using Graffle.FlowSdk.Services.Extensions;
using Graffle.FlowSdk.Types;
using System.Collections.Generic;

namespace Graffle.FlowSdk.Services
{
    public static class DictionaryTypeExtension
    {
        /// <summary>
        /// Return dictionary data as primitive types (string, int, etc) ie not objects of type FlowValueType
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Dictionary<dynamic, dynamic> ConvertToObject(this DictionaryType x, bool preserveKeyCasing = false)
        {
            var result = new Dictionary<dynamic, dynamic>();
            foreach (var item in x.Data)
            {
                string propertyName = ((dynamic)item.Key).Data.ToString(); //Data here is not guaranteed to be a string
                var cleanedName = preserveKeyCasing ? propertyName : propertyName.ToCamelCase();

                var value = item.Value;
                dynamic data = FlowValueTypeUtility.FlowTypeToPrimitive(value);

                result[cleanedName] = data;
            }

            return result;
        }
    }
}