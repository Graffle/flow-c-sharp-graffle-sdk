using System.Collections.Generic;
using System.Threading.Tasks;
using Graffle.FlowSdk.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Text.Json;
using Graffle.FlowSdk.Services.Nodes;
using System.Linq;
using Graffle.FlowSdk.Services.Models;
using System;

namespace Graffle.FlowSdk.Services.Tests.SerializationTests
{

    [TestClass]
    public class GraffleCompositeTypeConverterTests
    {
        [TestMethod]
        public void DeserializeFlowCadence_PrimitiveType_ReturnsGraffleCompositeType()
        {
            var cadenceJson = @"{""type"":""UInt32"",""value"":""112233""}";
            var uint32Field = CreateField("testName", cadenceJson);

            var fields = new List<Dictionary<string, string>>() { uint32Field };

            var converter = new GraffleCompositeTypeConverter();
            var result = converter.DeserializeFlowCadence("testId", "event", fields);

            Assert.IsNotNull(result);
            Assert.AreEqual("testId", result.Id);
            Assert.AreEqual("event", result.Type);

            var data = result.Data;
            Assert.AreEqual(1, data.Keys.Count);

            var key = data.Keys.First();
            Assert.AreEqual("testName", key);

            var value = data[key];

            Assert.AreEqual((uint)112233, value);
        }

        [TestMethod]
        public void DeserializeFlowCadence_PrimitiveType_Optional_ReturnsGraffleCompositeType()
        {
            var cadenceJson = $"{{\"type\":\"Optional\",\"value\":{{\"type\":\"Int16\",\"value\":123}}}}";
            var field = CreateField("testName", cadenceJson);

            var fields = new List<Dictionary<string, string>>() { field };

            var converter = new GraffleCompositeTypeConverter();
            var result = converter.DeserializeFlowCadence("testId", "event", fields);

            Assert.IsNotNull(result);
            Assert.AreEqual("testId", result.Id);
            Assert.AreEqual("event", result.Type);

            var data = result.Data;
            Assert.AreEqual(1, data.Keys.Count);

            var key = data.Keys.First();
            Assert.AreEqual("testName", key);

            var value = data[key];
            Assert.AreEqual(123, value);
        }

        [TestMethod]
        public void DeserializeFlowCadence_OptionalType_Null_ReturnsGraffleCompositeType()
        {
            var cadenceJson = "{\"type\":\"Optional\",\"value\":null}";
            var field = CreateField("testName", cadenceJson);

            var fields = new List<Dictionary<string, string>>() { field };

            var converter = new GraffleCompositeTypeConverter();
            var result = converter.DeserializeFlowCadence("testId", "event", fields);

            Assert.IsNotNull(result);
            Assert.AreEqual("testId", result.Id);
            Assert.AreEqual("event", result.Type);

            var data = result.Data;
            Assert.AreEqual(1, data.Keys.Count);

            var key = data.Keys.First();
            Assert.AreEqual("testName", key);

            var value = data[key];
            Assert.IsNull(value);
        }

        private Dictionary<string, string> CreateField(string name, string cadenceJson)
        {
            return new Dictionary<string, string>()
            {
                { "name", name},
                { "value", cadenceJson }
            };
        }
    }
}