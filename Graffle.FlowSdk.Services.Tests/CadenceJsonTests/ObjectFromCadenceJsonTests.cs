using Graffle.FlowSdk.Services.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Graffle.FlowSdk.Services.Tests.CadenceJsonTests;

[TestClass]
public class ObjectFromCadenceJsonTests
{
    [TestMethod]
    public void TopLevelArray_ThrowsArgumentException()
    {
        List<object> obj = ["abc", new { foo = "bar" }];
        var json = JsonSerializer.Serialize(obj);

        Assert.ThrowsException<ArgumentException>(() => CadenceJsonInterpreter.ObjectFromCadenceJson(json));
    }

    [TestMethod]
    [DataRow("foo")]
    [DataRow("")]
    [DataRow("      ")]
    public void InvalidJson_ThrowsArgumentException(string str)
    {
        Assert.ThrowsException<ArgumentException>(() => CadenceJsonInterpreter.ObjectFromCadenceJson(str));
    }

    [TestMethod]
    public void NullParam_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => CadenceJsonInterpreter.ObjectFromCadenceJson(default(string)));
    }

    [TestMethod]
    public void PreserveDictionaryKeyCasing_True_KeyCasingPreserved()
    {
        var json = @"{""type"":""Dictionary"",""value"":[{""key"":{""type"":""String"",""value"":""Foo""},""value"":{""type"":""String"",""value"":""test""}}]}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json, true);

        Assert.IsNotNull(res);
        if (res is not IDictionary<string, object> dict)
        {
            Assert.Fail("expected dictionary");
            return;
        }

        Assert.AreEqual(1, dict.Keys.Count);
        Assert.IsTrue(dict.ContainsKey("Foo"));
        Assert.IsFalse(dict.ContainsKey("foo"));
    }

    [TestMethod]
    public void PreserveDictionaryKeyCasing_False_LowercaseKey()
    {
        var json = @"{""type"":""Dictionary"",""value"":[{""key"":{""type"":""String"",""value"":""Foo""},""value"":{""type"":""String"",""value"":""test""}}]}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        Assert.IsNotNull(res);
        if (res is not IDictionary<string, object> dict)
        {
            Assert.Fail("expected dictionary");
            return;
        }

        Assert.AreEqual(1, dict.Keys.Count);
        Assert.IsFalse(dict.ContainsKey("Foo"));
        Assert.IsTrue(dict.ContainsKey("foo"));
    }
}
