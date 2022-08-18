using Graffle.FlowSdk.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

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
        public void ConvertToObject_CompositeValue_ReturnsDictionaryWithGraffleCompositeType()
        {
            var intField = new IntType(123);
            var stringField = new StringType("hello world");
            var structFields = new List<CompositeField>()
            {
                new CompositeField("intField", intField),
                new CompositeField("stringField", stringField)
            };
            var structData = new CompositeData("structId", structFields);

            var key = new StringType("keykeykeykeykey");
            var value = new CompositeType("Struct", structData);

            var dict = new DictionaryType(new Dictionary<FlowValueType, FlowValueType>() { { key, value } });

            var result = dict.ConvertToObject();
            Assert.IsInstanceOfType(result, typeof(Dictionary<dynamic, dynamic>));

            var resultDict = result as Dictionary<dynamic, dynamic>;

            Assert.AreEqual(1, resultDict.Count);

            var kvp = resultDict.First();
            Assert.IsInstanceOfType(kvp.Key, typeof(string));
            Assert.AreEqual(key.Data.ToCamelCase(), kvp.Key);

            Assert.IsInstanceOfType(kvp.Value, typeof(Dictionary<string, object>));

            var fields = kvp.Value as Dictionary<string, object>;
            Assert.AreEqual(2, fields.Count);

            //verify struct data
            var first = fields.First();
            Assert.AreEqual(structFields[0].Name.ToCamelCase(), first.Key);
            Assert.IsInstanceOfType(first.Value, typeof(int));
            Assert.AreEqual(intField.Data, first.Value);

            var second = fields.Skip(1).First();
            Assert.AreEqual(structFields[1].Name.ToCamelCase(), second.Key);
            Assert.IsInstanceOfType(second.Value, typeof(string));
            Assert.AreEqual(stringField.Data, second.Value);
        }

        [TestMethod]
        public void ConvertToObject_NestedArray_ReturnsDictionary()
        {
            var intType = new IntType(123);
            var stringType = new StringType("hello world");
            var arrayType = new ArrayType(new List<FlowValueType>() { intType, stringType });

            var dictKey = new StringType("dictionary KEY");

            var dictionaryType = new DictionaryType();
            dictionaryType.Data.Add(dictKey, arrayType);

            var result = dictionaryType.ConvertToObject();
            Assert.IsInstanceOfType(result, typeof(Dictionary<object, object>));
            var dict = result as Dictionary<object, object>;

            Assert.AreEqual(1, dict.Keys.Count);

            var item = dict.First();
            var key = item.Key;
            Assert.IsInstanceOfType(key, typeof(string)); //string ie not FlowValueType
            Assert.AreEqual(dictKey.Data, key);

            var value = item.Value;
            Assert.IsInstanceOfType(value, typeof(List<object>)); //dict key should be a list and not ArrayType

            var list = value as List<object>;
            Assert.AreEqual(2, list.Count);

            //need to verify that there are primitives in here and not FlowValueType objects
            var first = list[0];
            Assert.IsInstanceOfType(first, typeof(int));
            Assert.AreEqual(intType.Data, first);

            var second = list[1];
            Assert.IsInstanceOfType(second, typeof(string));
            Assert.AreEqual(stringType.Data, second);
        }

        [TestMethod]
        public void ConvertToObject_NestedDictionary_ReturnsDictionary() //yo dawg
        {
            var intType = new IntType(123);
            var stringType = new StringType("hello world");
            var innerDict = new DictionaryType();
            innerDict.Data.Add(intType, stringType);

            var dictKey = new StringType("dictionary KEY");
            var outerDict = new DictionaryType();
            outerDict.Data.Add(dictKey, innerDict);

            var result = outerDict.ConvertToObject(); //basically need to verify that there are no FlowValueType objects in here

            Assert.IsInstanceOfType(result, typeof(Dictionary<object, object>));
            var dict = result as Dictionary<object, object>;

            Assert.AreEqual(1, dict.Keys.Count);

            var item = dict.First();
            var key = item.Key;
            Assert.IsInstanceOfType(key, typeof(string)); //string ie not FlowValueType
            Assert.AreEqual(dictKey.Data, key);

            //verify data
            var value = item.Value;
            Assert.IsInstanceOfType(value, typeof(Dictionary<object, object>));
            var valueDict = value as Dictionary<object, object>;
            Assert.AreEqual(1, valueDict.Keys.Count);

            var innerItem = valueDict.First();
            var innerKey = innerItem.Key;
            Assert.IsInstanceOfType(innerKey, typeof(string));
            Assert.AreEqual(intType.Data.ToString(), innerKey);

            var innerValue = innerItem.Value;
            Assert.IsInstanceOfType(innerValue, typeof(string));
            Assert.AreEqual(stringType.Data, innerValue);
        }

        [TestMethod]
        public void ConvertToObject_PreserveKeyCasingTrue_KeyCasingPreserved()
        {
            const string key = "NFTKey";
            var stringType = new StringType(key);
            var intType = new IntType(5);

            var dictionaryType = new DictionaryType();
            dictionaryType.Data.Add(stringType, intType);

            var result = dictionaryType.ConvertToObject(true);

            Assert.IsNotNull(result);

            var resultKey = result.Keys.First();

            Assert.AreEqual(key, resultKey);
        }

        [TestMethod]
        public void ConvertToObject_PreserveKeyCasingFalse_KeyCasingNotPreserved()
        {
            const string key = "NFTKey";
            var stringType = new StringType(key);
            var intType = new IntType(5);

            var dictionaryType = new DictionaryType();
            dictionaryType.Data.Add(stringType, intType);

            var result = dictionaryType.ConvertToObject(); //default false

            Assert.IsNotNull(result);

            var resultKey = result.Keys.First();

            Assert.AreEqual(key.ToCamelCase(), resultKey);
        }
    }
}