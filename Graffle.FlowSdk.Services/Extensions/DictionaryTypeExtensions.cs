using Graffle.FlowSdk.Types;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Graffle.FlowSdk.Services
{
    public static class DictionaryTypeExtension
    {
        public static dynamic ConvertToObject(this DictionaryType x)
        {
            var result = new Dictionary<dynamic, dynamic>();
            foreach (var item in x.Data)
            {
                string propertyName = ((dynamic)item.Key).Data.ToString(); //Data here is not guaranteed to be a string
                var cleanedName = propertyName.ToCamelCase();

                var value = item.Value;
                dynamic data;
                if (FlowValueType.IsCompositeType(value.Type)) //nested composite type: struct, event, resource, etc
                {
                    var composite = value as CompositeType;
                    data = composite.FieldsAsDictionary();
                }
                else //primitive
                {
                    data = ((dynamic)item.Value).Data;
                }

                result[cleanedName] = data;
            }

            return result;
        }
    }
}