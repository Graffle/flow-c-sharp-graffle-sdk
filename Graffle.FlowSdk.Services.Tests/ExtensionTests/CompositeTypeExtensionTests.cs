using Graffle.FlowSdk.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Graffle.FlowSdk.Services.Tests.ExtensionTests
{
    [TestClass]
    public class CompositeTypeExtensionTests
    {
        [TestMethod]
        public void FieldsAsDictionary_ReturnsDictionary()
        {
            var intType = new Int16Type(123);
            var stringType = new StringType("hello");
            var fields = new List<CompositeField>()
            {
                new CompositeField("intField", intType),
                new CompositeField("stringField", stringType)
            };

            var composite = new CompositeType("Struct", "structId", fields);

            var result = composite.FieldsAsDictionary();
            Assert.IsNotNull(result);

            //verify data
            var first = result.First();
            Assert.AreEqual("intField", first.Key);
            Assert.AreEqual(first.Value, intType.Data);

            var second = result.Skip(1).First();
            Assert.AreEqual("stringField", second.Key);
            Assert.AreEqual(second.Value, stringType.Data);
        }
    }
}