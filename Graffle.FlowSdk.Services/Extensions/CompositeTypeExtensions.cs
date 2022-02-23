using Graffle.FlowSdk.Types;
using System.Collections.Generic;
using System.Linq;

namespace Graffle.FlowSdk.Services
{
    public static class CompositeTypeExtensions
    {
        public static Dictionary<string, dynamic> FieldsAsDictionary(this CompositeType composite)
        {
            return composite.Fields.ToDictionary(f => f.Name,
                    f =>
                    {
                        //todo: this is a massive hack that exists elsewhere in this sdk
                        //FlowValueType base class should have a way to expose this property
                        return ((dynamic)f.Value).Data;
                    });
        }
    }
}