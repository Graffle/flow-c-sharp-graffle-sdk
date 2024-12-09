using Graffle.FlowSdk.Services.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Graffle.FlowSdk.Services.Tests.CadenceJsonTests.ValueTests;

[TestClass]
public class ArrayTests
{
    [TestMethod]
    public void ArrayType()
    {
        var json = @"{""type"":""Array"",""value"": [ {""type"":""Int16"", ""value"":""123""}, {""type"":""String"",""value"":""hello world""}]}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        if (res is not List<object> values)
        {
            Assert.Fail("res not List<object>");
            return; //make compiler happy
        }

        //first item
        var first = values[0];
        Assert.AreEqual((long)123, first);

        //second item
        var second = values[1];
        Assert.AreEqual("hello world", second);
    }

    [TestMethod]
    [DataRow("Struct")]
    [DataRow("Resource")]
    [DataRow("Contract")]
    [DataRow("Event")]
    [DataRow("Enum")]
    public void ArrayType_NestedCompositeType(string nestedCompositeType)
    {
        /*
                    {
                        "type":"Array",
                        "value":[
                            {
                                "type":"Struct",
                                "value": {
                                    "id":"structId",
                                    "fields": [
                                        {
                                            "name":"structField1",
                                            "value": {
                                                "type":"Int",
                                                "value": "2"
                                            }
                                        }
                                    ]
                                }
                            }
                        ]
                    }
                    */

        var json = $"{{\"type\":\"Array\",\"value\":[{{\"type\":\"{nestedCompositeType}\",\"value\":{{\"id\":\"structId\",\"fields\":[{{\"name\":\"structField1\",\"value\":{{\"type\":\"Int\",\"value\":\"2\"}}}}]}}}}]}}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        if (res is not List<object> arr)
        {
            Assert.Fail("res not List<object>");
            return; //make compiler happy
        }

        Assert.AreEqual(1, arr.Count);

        var composite = arr[0] as IDictionary<string, object>;
        if (composite == null)
        {
            Assert.Fail("Expected dictionary");
            return;
        }

        Assert.AreEqual(1, composite.Count);

        var kvp = composite.First();
        Assert.AreEqual("structField1", kvp.Key);
        Assert.AreEqual(2, kvp.Value);
    }
}
