using Graffle.FlowSdk.Types;

namespace Graffle.FlowSdk.Services.Extensions
{
    public static class FlowValueTypeUtility
    {
        /// <summary>
        /// Break up a recursive FlowValueType and return an object containing its properties as primitive types (ie not cadence)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static dynamic FlowTypeToPrimitive(FlowValueType value)
        {
            if (FlowValueType.IsCompositeType(value.Type) && value is CompositeType composite)
            {
                return composite.FieldsAsDictionary();
            }
            else if (value.Type == "Array" && value is ArrayType array)
            {
                return array.ToValueData();
            }
            else if (value.Type == "Dictionary" && value is DictionaryType dict)
            {
                return dict.ConvertToObject();
            }
            else if (value.Type == "Optional" && value is OptionalType opt)
            {
                return opt.Data == null ? null : FlowTypeToPrimitive(opt.Data); //need to dig into the optional type
            }
            else //primitive
            {
                return ((dynamic)value).Data; //primitive value, just return the data directly
            }
        }
    }
}