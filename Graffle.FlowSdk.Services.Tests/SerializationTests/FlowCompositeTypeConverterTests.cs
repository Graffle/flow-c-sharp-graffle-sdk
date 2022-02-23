using Graffle.FlowSdk.Services.Models;
using Graffle.FlowSdk.Services.Nodes;
using Graffle.FlowSdk.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text.Json;
using System;
using System.Threading.Tasks;

namespace Graffle.FlowSdk.Services.Tests.SerializationTests
{
    [TestClass]
    public class FlowCompositeTypeConverterTests
    {
        [TestMethod]
        [DataRow("Struct")]
        [DataRow("Resource")]
        [DataRow("Event")]
        [DataRow("Contract")]
        [DataRow("Enum")]
        public void Read_ReturnsCompositeType(string type)
        {
            var json = $"{{\"type\":\"{type}\",\"value\":{{\"fields\":[{{\"name\":\"intField\",\"value\":{{\"type\":\"Int16\",\"value\":\"123\"}}}},{{\"name\":\"stringField\",\"value\":{{\"type\":\"String\",\"value\":\"hello\"}}}}],\"id\":\"idString\"}}}}";
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            var reader = new Utf8JsonReader(bytes);

            var converter = new FlowCompositeTypeConverter();
            var result = converter.Read(ref reader, null, null);

            Assert.IsNotNull(result);
            Assert.AreEqual(type, result.Type);
            Assert.AreEqual("idString", result.Id);

            var data = result.Data;
            Assert.IsNotNull(data);

            var id = data.Id;
            Assert.AreEqual("idString", id);

            var fields = data.Fields;
            Assert.IsNotNull(fields);
            Assert.AreEqual(2, fields.Count);

            //verify data
            var firstName = fields[0].Name;
            Assert.AreEqual("intField", firstName);

            var firstValue = fields[0].Value;
            Assert.IsNotNull(firstValue);
            Assert.IsInstanceOfType(firstValue, typeof(Int16Type));

            var intType = firstValue as Int16Type;
            Assert.AreEqual(123, intType.Data);

            var secondName = fields[1].Name;
            Assert.AreEqual("stringField", secondName);

            var secondValue = fields[1].Value;
            Assert.IsNotNull(secondValue);
            Assert.IsInstanceOfType(secondValue, typeof(StringType));

            var stringType = secondValue as StringType;
            Assert.AreEqual("hello", stringType.Data);
        }

        [TestMethod]
        public void Write_ThrowsNotImplementedException()
        {
            var converter = new FlowCompositeTypeConverter();
            Assert.ThrowsException<NotImplementedException>(() => converter.Write(null, null, null));
        }
    }
}