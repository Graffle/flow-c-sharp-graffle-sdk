using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Graffle.FlowSdk.Types.TypeDefinitions;

namespace Graffle.FlowSdk.Services.Serialization
{
    public class CadenceTypeInterpreter
    {
        public static dynamic InterpretCadenceType(object cadenceObject)
        {
            if (cadenceObject is string str)
            {
                return cadenceObject; //repeated type TODO test and comment!!!
            }

            if (cadenceObject is not IDictionary<string, object> type) //expecting ExpandoObject object here
                throw new Exception("todo");

            var kind = type["kind"].ToString();
            Dictionary<string, object> result = new() { { "kind", kind } };

            switch (kind)
            {
                //composite types
                case "Resource":
                case "Struct":
                case "Event":
                case "Contract":
                case "StructInterface":
                case "ResourceInterface":
                case "ContractInterface":
                    {
                        result.Add("type", string.Empty);
                        result.Add("typeID", type["typeID"]);
                        result.Add("initializers", GetInitializers(type["initializers"]));
                        result.Add("fields", GetFields(type["fields"]));
                        break;
                    }
                case "Capability":
                    {
                        var capabilityType = type["type"];
                        result.Add("type", capabilityType == null ? string.Empty : InterpretCadenceType(capabilityType));
                        break;
                    }
                case "Dictionary":
                    {
                        result.Add("key", InterpretCadenceType(type["key"]));
                        result.Add("value", InterpretCadenceType(type["value"]));
                        break;
                    }
                case "Reference":
                    {
                        result.Add("authorized", type["authorized"]); //should be bool
                        result.Add("type", InterpretCadenceType(type["type"]));
                        break;
                    }
                case "Optional":
                    {
                        result.Add("type", InterpretCadenceType(type["type"]));
                        break;
                    }
                case "Intersection":
                    {
                        result.Add("typeID", type["typeID"]);
                        if (type["types"] is not List<object> types)
                        {
                            throw new Exception("todo");
                        }

                        result.Add("types", types.Select(InterpretCadenceType).ToList());
                        break;
                    }
                case "Restriction":
                    {
                        result.Add("typeID", type["typeID"]);
                        result.Add("type", InterpretCadenceType(type["type"]));

                        if (type["restrictions"] is not List<object> restrictions)
                        {
                            throw new Exception("todo");
                        }


                        result.Add("restrictions", restrictions.Select(InterpretCadenceType).ToList());
                        break;
                    }
                case "VariableSizedArray":
                    {
                        result.Add("type", InterpretCadenceType(type["type"]));
                        break;
                    }
                case "ConstantSizedArray":
                    {
                        result.Add("size", type["size"]);
                        result.Add("type", InterpretCadenceType(type["type"]));
                        break;
                    }
                case "Enum":
                    {
                        result.Add("type", InterpretCadenceType(type["type"]));
                        result.Add("typeID", type["typeID"]);
                        result.Add("initializers", GetInitializers(type["initializers"]));
                        result.Add("fields", GetFields(type["fields"]));
                        break;
                    }
                case "Function":
                    {
                        result.Add("typeID", type["typeID"]);
                        if (type["parameters"] is not IList<object> parameters)
                        {
                            throw new Exception("todo");
                        }
                        result.Add("parameters", parameters.Select(GetParameter).ToList());
                        result.Add("return", InterpretCadenceType(type["return"]));
                        break;
                    }
                case "Int":
                case "Int8":
                case "Int16":
                case "Int32":
                case "Int64":
                case "Int128":
                case "Int256":
                case "UInt":
                case "UInt8":
                case "UInt16":
                case "UInt32":
                case "UInt64":
                case "UInt128":
                case "UInt256":
                case "Word8":
                case "Word16":
                case "Word32":
                case "Word64":
                case "Fix64":
                case "UFix64":
                case "Bool":
                case "String":
                case "Address":
                case "Any":
                case "AnyStruct":
                case "AnyResource":
                case "Type":
                case "Void":
                case "Never":
                case "Character":
                case "Bytes":
                case "Number":
                case "SignedNumber":
                case "Integer":
                case "SignedInteger":
                case "FixedPoint":
                case "SignedFixedPoint":
                case "Path":
                case "CapabilityPath":
                case "StoragePath":
                case "PublicPath":
                case "PrivatePath":
                case "AuthAccount":
                case "PublicAccount":
                case "AuthAccount.Keys":
                case "PublicAccount.Keys":
                case "AuthAccount.Contracts":
                case "PublicAccount.Contracts":
                case "DeployedContract":
                case "AccountKey":
                case "Block":
                    {
                        break;
                    }
                default:
                    throw new Exception("todo");
            }

            return result;
        }

        public static IDictionary<string, object> GetParameter(object value)
        {
            if (value is not IDictionary<string, object> dict)
                throw new Exception("todo");

            return new Dictionary<string, object>()
            {
                { "label", dict["label"] },
                { "id", dict["id"] },
                { "type", InterpretCadenceType(dict["type"]) }
            };
        }

        public static List<object> GetInitializers(object value)
        {
            if (value is not IList<object> list)
                throw new Exception("todo");

            return list.Select(GetParameter).Cast<object>().ToList();
        }

        public static IList<object> GetFields(object value)
        {
            if (value is not IList<object> list)
                throw new Exception("todo");

            return list.Select(GetField).Cast<object>().ToList();
        }

        public static IDictionary<string, object> GetField(object value)
        {
            if (value is not IDictionary<string, object> dict)
                throw new Exception("todo");

            return new Dictionary<string, object>
            {
                { "id", dict["id"] },
                { "type", InterpretCadenceType(dict["type"]) }
            };
        }
    }
}