using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Graffle.FlowSdk.Types.TypeDefinitions;

namespace Graffle.FlowSdk.Services.Serialization
{
    public class CadenceTypeParser
    {
        public static dynamic ParseFlowType(object incoming)
        {
            if (incoming is string str)
            {
                return incoming; //repeated type TODO test and comment!!!
            }

            if (incoming is not IDictionary<string, object> typeDict) //expecting ExpandoObject object here {}
                throw new Exception("todo");

            var kind = typeDict["kind"].ToString();

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
                        result.Add("typeID", typeDict["typeID"]);
                        result.Add("initializers", ParseInitializers(typeDict["initializers"]));
                        result.Add("fields", ParseFields(typeDict["fields"]));
                        break;
                    }
                case "Capability":
                    {
                        var type = result["type"];
                        result.Add("type", type == null ? string.Empty : ParseFlowType(type));
                        break;
                    }
                case "Dictionary":
                    {
                        result.Add("key", ParseFlowType(typeDict["key"]));
                        result.Add("value", ParseFlowType(typeDict["value"]));
                        break;
                    }
                case "Reference":
                    {
                        result.Add("authorized", typeDict["authorized"]); //should be bool
                        result.Add("type", ParseFlowType(typeDict["type"]));
                        break;
                    }
                case "Optional":
                    {
                        result.Add("type", ParseFlowType(typeDict["type"]));
                        break;
                    }
                case "Intersection":
                    {
                        result.Add("typeID", typeDict["typeID"]);
                        if (typeDict["types"] is not List<object> types)
                        {
                            throw new Exception("todo");
                        }

                        result.Add("types", types.Select(x => ParseFlowType(x)).ToList());
                        break;
                    }
                case "Restriction":
                    {
                        result.Add("typeID", typeDict["typeID"]);
                        result.Add("type", ParseFlowType(typeDict["type"]));

                        if (typeDict["restrictions"] is not List<object> restrictions)
                        {
                            throw new Exception("todo");
                        }


                        result.Add("restrictions", restrictions.Select(x => ParseFlowType(x)).ToList());
                        break;
                    }
                case "VariableSizedArray":
                    {
                        result.Add("type", ParseFlowType(typeDict["type"]));
                        break;
                    }
                case "ConstantSizedArray":
                    {
                        result.Add("size", typeDict["size"]);
                        result.Add("type", ParseFlowType(typeDict["type"]));
                        break;
                    }
                case "Enum":
                    {
                        result.Add("type", ParseFlowType(typeDict["type"]));
                        result.Add("typeID", typeDict["typeID"]);
                        result.Add("initializers", ParseInitializers(typeDict["initializers"]));
                        result.Add("fields", ParseFields(typeDict["fields"]));
                        break;
                    }
                case "Function":
                    {
                        result.Add("typeID", typeDict["typeID"]);
                        if (typeDict["parameters"] is not IList<object> parameters)
                        {
                            throw new Exception("todo");
                        }
                        result.Add("parameters", parameters.Select(x => ParseParameterType(x)).ToList());
                        result.Add("return", ParseFlowType(typeDict["return"]));
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
                        result.Add("kind", kind);
                        break;
                    }
                default:
                    throw new Exception("todo");
            }

            return result;
        }

        public static IDictionary<string, object> ParseParameterType(object value)
        {
            if (value is not IDictionary<string, object> dict)
                throw new Exception("todo");

            Dictionary<string, object> result = [];

            result.Add("label", dict["label"]);
            result.Add("id", dict["id"]);
            result.Add("type", ParseFlowType(dict["type"]));
            return result;
        }

        public static List<object> ParseInitializers(object value)
        {
            if (value is not IList<object> list)
                throw new Exception("todo");

            List<object> result = [];
            foreach (var item in list)
            {
                result.Add(ParseParameterType(item));
            }

            return result;
        }

        public static IList<object> ParseFields(object value)
        {
            if (value is not IList<object> list)
                throw new Exception("todo");

            List<object> result = [];
            foreach (var item in list)
            {
                result.Add(ParseFieldType(item));
            }

            return result;
        }

        public static IDictionary<string, object> ParseFieldType(object value)
        {
            if (value is not IDictionary<string, object> dict)
                throw new Exception("todo");

            Dictionary<string, object> result = [];

            result.Add("id", dict["id"]);
            result.Add("type", ParseFlowType(dict["type"]));
            return result;
        }
    }
}