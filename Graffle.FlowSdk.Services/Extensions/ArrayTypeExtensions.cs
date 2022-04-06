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
                if (FlowValueType.IsCompositeType(item.Type) && item is CompositeType composite) //complex type (struct, event), get the fields
                {
                    result.Add(composite.FieldsAsDictionary());
                }
                else if (item.Type == "Dictionary" && item is DictionaryType dictionary) //get the dictionary data as primitive values
                {
                    result.Add(dictionary.ConvertToObject());
                }
                else if (item.Type == "Array" && item is ArrayType arr) //nested array, need to recurse
                {
                    result.Add(arr.ToValueData());
                }
                else //primitive
                {
                    result.Add(((dynamic)item).Data); //primitive type, just add the Data directly
                }
            }
            return result;
        }
    }
}