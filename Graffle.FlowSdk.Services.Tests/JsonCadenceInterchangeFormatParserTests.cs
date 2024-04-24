using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Graffle.FlowSdk.Services.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graffle.FlowSdk.Services.Tests
{
    [TestClass]
    public class JsonCadenceInterchangeFormatParserTests
    {
        [TestMethod]
        public void Dict()
        {
            var cadenceJsonString = @"{""type"":""Dictionary"",""value"":[{""key"":{""type"":""UInt8"",""value"":""123""},""value"":{""type"":""String"",""value"":""test""}}]}";

            var res = JsonCadenceInterchangeFormatParser.FromJson(cadenceJsonString);
        }

        [TestMethod]
        [DataRow("Struct")]
        [DataRow("Resource")]
        [DataRow("Event")]
        [DataRow("Contract")]
        [DataRow("Enum")]
        public void composite(string type)
        {
            var expectedJson = $"{{\"type\":\"{type}\",\"value\":{{\"fields\":[{{\"name\":\"intField\",\"value\":{{\"type\":\"Int16\",\"value\":\"123\"}}}},{{\"name\":\"stringField\",\"value\":{{\"type\":\"String\",\"value\":\"hello\"}}}}],\"id\":\"idString\"}}}}";

            var res = JsonCadenceInterchangeFormatParser.FromJson(expectedJson);
        }

        [TestMethod]
        public void composite_type()
        {
            var expectedJson = "{\"kind\":\"Resource\",\"typeID\":\"compositeTypeId\",\"fields\":[{\"id\":\"fieldId\",\"type\":{\"kind\":\"Int32\"}}],\"initializers\":[],\"type\":\"\"}";

            var res = JsonCadenceInterchangeFormatParser.TypeFromJson(expectedJson);
        }

    }
}