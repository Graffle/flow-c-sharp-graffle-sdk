using Graffle.FlowSdk.Services.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graffle.FlowSdk.Services.Tests.CadenceJsonTests.ValueTests;

[TestClass]
public class IntegerTests
{
    [TestMethod]
    public void Int8()
    {
        var json = @"{""type"":""Int8"",""value"":""0""}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        Assert.AreEqual(0L, res);
    }

    [TestMethod]
    public void Int16()
    {
        var json = @"{""type"":""Int16"",""value"":""0""}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        Assert.AreEqual(0L, res);
    }

    [TestMethod]
    public void Int32()
    {
        var json = @"{""type"":""Int32"",""value"":""0""}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        Assert.AreEqual(0L, res);
    }

    [TestMethod]
    public void Int64()
    {
        var json = @"{""type"":""Int64"",""value"":""0""}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        Assert.AreEqual(0L, res);
    }

    [TestMethod]
    public void Int128()
    {
        var json = @"{""type"":""Int128"",""value"":""91389681247993671255432112000000000""}";

        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);
        Assert.AreEqual("91389681247993671255432112000000000", res);
    }

    [TestMethod]
    public void Int256()
    {
        var json = @"{""type"":""Int256"",""value"":""91389681247993671255432112000000000""}";

        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);
        Assert.AreEqual("91389681247993671255432112000000000", res);
    }

    [TestMethod]
    public void IntType()
    {
        var json = @"{""type"":""Int"",""value"":""0""}";

        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);
        Assert.AreEqual(0, res);
    }

    [TestMethod]
    public void IntType_BigNumber()
    {
        var json = @"{""type"":""Int"",""value"":""91389681247993671255432112000000000""}";

        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);
        Assert.AreEqual("91389681247993671255432112000000000", res);
    }

    [TestMethod]
    public void UInt8()
    {
        var json = @"{""type"":""UInt8"",""value"":""0""}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        Assert.AreEqual(0ul, res);
    }

    [TestMethod]
    public void UInt16()
    {
        var json = @"{""type"":""UInt16"",""value"":""0""}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        Assert.AreEqual(0ul, res);
    }

    [TestMethod]
    public void UInt32()
    {
        var json = @"{""type"":""UInt32"",""value"":""0""}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        Assert.AreEqual(0ul, res);
    }

    [TestMethod]
    public void UInt64()
    {
        var json = @"{""type"":""UInt64"",""value"":""0""}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        Assert.AreEqual(0ul, res);
    }

    [TestMethod]
    public void UInt128()
    {
        var json = @"{""type"":""UInt128"",""value"":""0""}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        Assert.AreEqual("0", res);
    }

    [TestMethod]
    public void UInt256()
    {
        var json = @"{""type"":""UInt256"",""value"":""0""}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        Assert.AreEqual("0", res);
    }

    [TestMethod]
    public void UInt()
    {
        var json = @"{""type"":""UInt"",""value"":""0""}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        Assert.AreEqual(0U, res);
    }

    [TestMethod]
    public void Word8()
    {
        var json = @"{""type"":""Word8"",""value"":""100""}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        Assert.AreEqual(100ul, res);
    }

    [TestMethod]
    public void Word16()
    {
        var json = @"{""type"":""Word16"",""value"":""100""}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        Assert.AreEqual(100ul, res); ;
    }

    [TestMethod]
    public void Word32()
    {
        var json = @"{""type"":""Word32"",""value"":""100""}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        Assert.AreEqual(100ul, res);
    }

    [TestMethod]
    public void Word64()
    {
        var json = @"{""type"":""Word32"",""value"":""100""}";
        var res = CadenceJsonInterpreter.ObjectFromCadenceJson(json);

        Assert.AreEqual(100ul, res);
    }
}
