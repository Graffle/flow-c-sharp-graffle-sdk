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
                    var flowComposite = value as CompositeType;
                    var graffleComposite = new GraffleCompositeType(value.Type);
                    graffleComposite.Id = flowComposite.Id;
                    graffleComposite.Data = flowComposite.Fields.ToDictionary(f => f.Name, f =>
                    {
                        //todo: this is a massive hack that exists elsewhere in this sdk
                        //FlowValueType base class should have a way to expose this property
                        return ((dynamic)f.Value).Data;
                    });
                    data = graffleComposite;
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