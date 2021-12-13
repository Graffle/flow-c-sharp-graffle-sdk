using System.Collections.Generic;
using Graffle.FlowSdk.Types;

namespace Graffle.FlowSdk.Services 
{
    public static class DictionaryTypeExtension
    {
        public static dynamic ConvertToObject(this DictionaryType x)
        {
            var result = new Dictionary<dynamic, dynamic>();
            foreach (var item in x.Data)
            {
                string propertyName = ((dynamic)item.Key).Data;
                var cleanedName = propertyName.ToCamelCase();
                result[cleanedName] = ((dynamic)item.Value).Data;
            }
            return result;
        }
    }
}