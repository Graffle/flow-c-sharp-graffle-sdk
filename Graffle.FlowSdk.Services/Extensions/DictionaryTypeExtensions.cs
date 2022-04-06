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
        public static Dictionary<dynamic, dynamic> ConvertToObject(this DictionaryType x)
        {
            var result = new Dictionary<dynamic, dynamic>();
            foreach (var item in x.Data)
            {
                string propertyName = ((dynamic)item.Key).Data.ToString(); //Data here is not guaranteed to be a string
                var cleanedName = propertyName.ToCamelCase();

                var value = item.Value;
                dynamic data;
                if (FlowValueType.IsCompositeType(value.Type) && value is CompositeType composite) //nested composite type: struct, event, resource, etc
                {
                    data = composite.FieldsAsDictionary();
                }
                else if (value.Type == "Array" && value is ArrayType array) //nested array, need to get the values as primitive
                {
                    data = array.ToValueData();
                }
                else if (value.Type == "Dictionary" && value is DictionaryType dictionary) //nested dictionary, need to recurse for primitive values
                {
                    data = dictionary.ConvertToObject();
                }
                else //primitive (int, string)
                {
                    data = ((dynamic)item.Value).Data; //primitive type, access the data directly
                }

                result[cleanedName] = data;
            }

            return result;
        }
    }
}