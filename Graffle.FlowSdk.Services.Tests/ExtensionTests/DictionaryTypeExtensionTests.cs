using Graffle.FlowSdk.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Graffle.FlowSdk.Services.Tests.ExtensionTests
{
    [TestClass]
    public class DictionaryTypeExtensionTests
    {
        [TestMethod]
        public void ConvertToObject_StringKey_ReturnsDictionary()
        {
            var key = new StringType("keykeykey");
            var value = new UFix64Type(12345.65m);
            var dict = new DictionaryType(new Dictionary<FlowValueType, FlowValueType>() { { key, value } });

            var result = dict.ConvertToObject();
            Assert.IsInstanceOfType(result, typeof(Dictionary<dynamic, dynamic>));

            var resultDict = result as Dictionary<dynamic, dynamic>;
            Assert.AreEqual(1, resultDict.Count());

            var kvp = resultDict.First();
            Assert.IsInstanceOfType(kvp.Key, typeof(string));
            Assert.AreEqual(key.Data.ToCamelCase(), kvp.Key);

            Assert.IsInstanceOfType(kvp.Value, typeof(decimal));
            Assert.AreEqual(value.Data, kvp.Value);
        }

        [TestMethod]
        public void ConvertToObject_IntKey_ReturnsDictionary()
        {
            var key = new IntType(1122);
            var value = new UFix64Type(12345.65m);
            var dict = new DictionaryType(new Dictionary<FlowValueType, FlowValueType>() { { key, value } });

            var result = dict.ConvertToObject();
            Assert.IsInstanceOfType(result, typeof(Dictionary<dynamic, dynamic>));

            var resultDict = result as Dictionary<dynamic, dynamic>;
            Assert.AreEqual(1, resultDict.Count());

            var kvp = resultDict.First();
            Assert.IsInstanceOfType(kvp.Key, typeof(string)); //converted to string
            Assert.AreEqual(key.Data.ToString(), kvp.Key);

            Assert.IsInstanceOfType(kvp.Value, typeof(decimal));
            Assert.AreEqual(12345.65m, kvp.Value);
        }

        [TestMethod]
        public void ConvertToObject_StructValue_ReturnsDictionary()
        {
            var intField = new IntType(123);
            var stringField = new StringType("hello world");
            var structFields = new List<StructField>()
            {
                new StructField("intField", intField),
                new StructField("stringField", stringField)
            };
            var structData = new StructData("structId", structFields);

            var key = new StringType("keykeykeykeykey");
            var value = new StructType(structData);

            var dict = new DictionaryType(new Dictionary<FlowValueType, FlowValueType>() { { key, value } });

            var result = dict.ConvertToObject();
            Assert.IsInstanceOfType(result, typeof(Dictionary<dynamic, dynamic>));

            var resultDict = result as Dictionary<dynamic, dynamic>;

            Assert.AreEqual(1, resultDict.Count);

            var kvp = resultDict.First();
            Assert.IsInstanceOfType(kvp.Key, typeof(string));
            Assert.AreEqual(key.Data.ToCamelCase(), kvp.Key);

            Assert.IsInstanceOfType(kvp.Value, typeof(Dictionary<string, dynamic>));

            var structDict = kvp.Value as Dictionary<string, dynamic>;
            Assert.AreEqual(2, structDict.Count);

            //verify struct data
            var first = structDict.First();
            Assert.AreEqual(structFields[0].Name.ToCamelCase(), first.Key);
            Assert.IsInstanceOfType(first.Value, typeof(int));
            Assert.AreEqual(intField.Data, first.Value);

            var second = structDict.Skip(1).First();
            Assert.AreEqual(structFields[1].Name.ToCamelCase(), second.Key);
            Assert.IsInstanceOfType(second.Value, typeof(string));
            Assert.AreEqual(stringField.Data, second.Value);
        }
    }
}