using Graffle.FlowSdk.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Graffle.FlowSdk.Services.Tests.ExtensionTests
{
    [TestClass]
    public class StructTypeExtensionTests
    {
        [TestMethod]
        public void ConvertToObject_ReturnsDictionary()
        {
            var intField = new IntType(123);
            var stringField = new StringType("hello world");
            var structFields = new List<StructField>()
            {
                new StructField("intField", intField),
                new StructField("stringField", stringField)
            };
            var structData = new StructData("structId", structFields);
            var flowStruct = new StructType(structData);

            var result = flowStruct.ConvertToObject();
            Assert.IsInstanceOfType(result, typeof(Dictionary<string, dynamic>));

            var resultDict = result as Dictionary<string, dynamic>;
            Assert.AreEqual(2, resultDict.Count);

            //verify struct data
            var first = resultDict.First();
            Assert.AreEqual(structFields[0].Name.ToCamelCase(), first.Key);
            Assert.IsInstanceOfType(first.Value, typeof(int));
            Assert.AreEqual(intField.Data, first.Value);

            var second = resultDict.Skip(1).First();
            Assert.AreEqual(structFields[1].Name.ToCamelCase(), second.Key);
            Assert.IsInstanceOfType(second.Value, typeof(string));
            Assert.AreEqual(stringField.Data, second.Value);
        }
    }
}