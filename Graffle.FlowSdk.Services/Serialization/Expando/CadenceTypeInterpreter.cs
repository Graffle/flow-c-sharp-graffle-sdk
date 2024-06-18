using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Graffle.FlowSdk.Services.Serialization
{
    public static class CadenceTypeInterpreter
    {
        private static readonly ExpandoObjectConverter _expando = new();

        /// <summary>
        /// IDictionary<string,object>, ie json object
        /// </summary>
        /// <returns></returns>
        private static readonly Type OBJECT_TYPE = typeof(IDictionary<string, object>);

        /// <summary>
        /// IList<object>, ie json array
        /// </summary>
        /// <returns></returns>
        private static readonly Type ARRAY_TYPE = typeof(IList<object>);

        public static dynamic ObjectFromCadenceJson(string json)
        {
            var parsed = JsonConvert.DeserializeObject<ExpandoObject>(json, _expando);

            return InterpretCadenceType(parsed);
        }

        public static dynamic InterpretCadenceType(object cadenceType)
        {
            if (cadenceType is string)
            {
                //repeated type or old cadence type (pre secure cadence)
                return cadenceType;
            }

            ArgumentNullException.ThrowIfNull(cadenceType, nameof(cadenceType));
            if (cadenceType is not IDictionary<string, object> type) //expecting ExpandoObject object here
            {
                throw new CadenceJsonCastException("Invalid type for Cadence type object")
                {
                    ExpectedType = OBJECT_TYPE,
                    ActualType = cadenceType?.GetType()
                };
            }

            var kind = type["kind"].ToString();
            Dictionary<string, dynamic> result = new() { { "kind", kind } };
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
                        if (type.TryGetValue("authorized", out var authorized)) //backwards compat
                        {
                            result.Add("authorized", authorized); //should be bool
                        }
                        else if (type.TryGetValue("authorization", out var authorization)) //cadence 1.0
                        {
                            result.Add("authorization", GetAuthorization(authorization));
                        }

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
                            throw new CadenceJsonCastException($"Unexpected type for Intersection \"types\" field")
                            {
                                ExpectedType = ARRAY_TYPE,
                                ActualType = type["types"]?.GetType()
                            };
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
                            throw new CadenceJsonCastException("Unexpected type for Restriction \"restrictions\" field")
                            {
                                ExpectedType = ARRAY_TYPE,
                                ActualType = type["restrictions"]?.GetType()
                            };
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
                        result.Add("type", InterpretCadenceType(type["type"]));
                        result.Add("size", type["size"]);
                        break;
                    }
                case "Enum":
                    {
                        result.Add("type", InterpretCadenceType(type["type"]));
                        result.Add("typeID", type["typeID"]);
                        result.Add("fields", GetFields(type["fields"]));
                        result.Add("initializers", GetInitializers(type["initializers"]));
                        break;
                    }
                case "Function":
                    {
                        result.Add("typeID", type["typeID"]);

                        if (type["parameters"] is not IList<object> parameters)
                        {
                            throw new CadenceJsonCastException($"Unexpected type for Function \"parameters\" field")
                            {
                                ExpectedType = ARRAY_TYPE,
                                ActualType = type["parameters"]?.GetType()
                            };
                        }

                        result.Add("parameters", parameters.Select(GetParameter).ToList());
                        result.Add("return", InterpretCadenceType(type["return"]));
                        break;
                    }
                case "InclusiveRange":
                    {
                        result.Add("element", InterpretCadenceType(type["element"]));
                        break;
                    }
                case "Any":
                case "AnyStruct":
                case "AnyResource":
                case "AnyStructAttachment":
                case "AnyResourceAttachment":
                case "Type":
                case "Void":
                case "Never":
                case "Bool":
                case "String":
                case "Character":
                case "Bytes":
                case "Address":
                case "Number":
                case "SignedNumber":
                case "Integer":
                case "SignedInteger":
                case "FixedPoint":
                case "SignedFixedPoint":
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
                        break; //result just contains "kind" : "Type"
                    }
                default:
                    throw new Exception($"Unknown kind {kind}");
            }

            return result;
        }

        public static dynamic GetAuthorization(object value)
        {
            if (value is not IDictionary<string, object> authorization)
            {
                throw new CadenceJsonCastException("Unexpected type received for Authorization object")
                {
                    ExpectedType = OBJECT_TYPE,
                    ActualType = value?.GetType()
                };
            }

            var res = new Dictionary<string, dynamic>()
            {
                {"kind", authorization["kind"]}
            };

            if (authorization["entitlements"] is not IList<object> entitlements)
            {
                throw new CadenceJsonCastException("Unexpected type received for Authorization entitlements object")
                {
                    ExpectedType = ARRAY_TYPE,
                    ActualType = authorization["entitlements"]?.GetType()
                };
            }

            var parsedEntitlements = new List<dynamic>();
            foreach (var ent in entitlements)
            {
                if (ent is not IDictionary<string, object> entitlement)
                {
                    throw new CadenceJsonCastException($"Unexpected type received for entitlement object")
                    {
                        ExpectedType = OBJECT_TYPE,
                        ActualType = ent?.GetType()
                    };
                }

                parsedEntitlements.Add(new Dictionary<string, dynamic>()
                {
                    { "kind", entitlement["kind"]},
                    { "typeID", entitlement["typeID"]}
                });
            }
            res["entitlements"] = parsedEntitlements;

            return res;
        }

        public static dynamic GetParameter(object value)
        {
            if (value is not IList<object> list)
            {
                throw new CadenceJsonCastException($"Unexpected type for Parameter object")
                {
                    ExpectedType = ARRAY_TYPE,
                    ActualType = value?.GetType()
                };
            }

            var res = new Dictionary<string, dynamic>();
            foreach (var item in list)
            {
                if (item is not IDictionary<string, object> p)
                {
                    throw new CadenceJsonCastException("Unexpected type for inner Parameter object")
                    {
                        ExpectedType = OBJECT_TYPE,
                        ActualType = item?.GetType()
                    };
                }

                res.Add("label", p["label"]);
                res.Add("id", p["id"]);
                res.Add("type", InterpretCadenceType(p["type"]));
            }
            return res;
        }

        public static dynamic GetInitializers(object value)
        {
            if (value is not IList<object> list)
            {
                throw new CadenceJsonCastException("Unexpected type for GetInitializers object")
                {
                    ExpectedType = ARRAY_TYPE,
                    ActualType = value?.GetType()
                };
            }

            return list.Select(GetParameter).ToList();
        }

        public static dynamic GetFields(object value)
        {
            if (value is not IList<object> list)
            {
                throw new CadenceJsonCastException("Unexpected type for GetFields object")
                {
                    ExpectedType = ARRAY_TYPE,
                    ActualType = value?.GetType()
                };
            }

            return list.Select(GetField).ToList();
        }

        public static dynamic GetField(object value)
        {
            if (value is not IDictionary<string, object> dict)
            {
                throw new CadenceJsonCastException("Unexpected type for GetField object")
                {
                    ExpectedType = OBJECT_TYPE,
                    ActualType = value?.GetType()
                };
            }

            return new Dictionary<string, dynamic>
            {
                { "id", dict["id"] },
                { "type", InterpretCadenceType(dict["type"]) }
            };
        }
    }
}