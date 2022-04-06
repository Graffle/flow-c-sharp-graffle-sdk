using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Graffle.FlowSdk.Services.Extensions;
using Graffle.FlowSdk.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace Graffle.FlowSdk.Services.Tests.ExtensionTests
{
    [TestClass]
    public class ArrayExtensionsTests
    {
        [TestMethod]
        public void ToValueData_PrimitiveValues_ReturnsList()
        {
            var intType = new Int16Type(123);
            var ufix64Type = new UFix64Type(2345.6m);
            var arrayType = new ArrayType(new List<FlowValueType>() { intType, ufix64Type });

            var result = arrayType.ToValueData();
            Assert.IsInstanceOfType(result, typeof(List<object>));

            var list = result as List<object>;
            Assert.AreEqual(2, list.Count);

            Assert.AreEqual(intType.Data, list[0]);
            Assert.AreEqual(ufix64Type.Data, list[1]);
        }

        [TestMethod]
        public void ToValueData_NestedArray_ReturnsList()
        {
            var intType = new Int16Type(123);
            var ufix64Type = new UFix64Type(2345.6m);
            var innerArrayType = new ArrayType(new List<FlowValueType>() { intType, ufix64Type });

            //arraytype with nested arraytype
            var outerArrayType = new ArrayType(new List<FlowValueType>() { innerArrayType });

            var result = outerArrayType.ToValueData();
            Assert.IsInstanceOfType(result, typeof(List<object>));

            var list = result as List<object>;
            Assert.AreEqual(1, list.Count);

            //pull out the outer array
            var item = list.First();
            Assert.IsInstanceOfType(item, typeof(List<object>));

            //pull out the inner array
            //here we are verifying that the list *does not* contain FlowValueType
            var innerList = item as List<object>;
            Assert.AreEqual(2, innerList.Count);

            foreach (var x in innerList)
            {
                Assert.IsNotInstanceOfType(x, typeof(FlowValueType));
            }

            Assert.AreEqual(intType.Data, innerList[0]);
            Assert.AreEqual(ufix64Type.Data, innerList[1]);
        }

        [TestMethod]
        public void ToValueData_NestedDictionary_ReturnsList()
        {
            var intType = new Int16Type(123);
            var ufix64Type = new UFix64Type(2345.6m);
            var dictionaryType = new DictionaryType();
            dictionaryType.Data.Add(intType, ufix64Type);

            //nest this dictionary inside of an array
            var arrayType = new ArrayType(new List<FlowValueType>() { dictionaryType });

            var result = arrayType.ToValueData();
            Assert.IsInstanceOfType(result, typeof(List<object>));

            var list = result as List<object>;
            Assert.AreEqual(1, list.Count());

            //pull out the nested dictionary 
            var item = list.First();
            Assert.IsNotInstanceOfType(item, typeof(FlowValueType)); //should NOT be FlowValueType

            Assert.IsInstanceOfType(item, typeof(Dictionary<object, object>));
            var dict = item as Dictionary<object, object>;

            Assert.AreEqual(1, dict.Keys.Count);

            var dictItem = dict.First();
            Assert.AreEqual(intType.Data.ToString(), dictItem.Key); //key gets converted to string
            Assert.AreEqual(ufix64Type.Data, dictItem.Value);

            Assert.IsNotInstanceOfType(dictItem.Key, typeof(FlowValueType));
            Assert.IsNotInstanceOfType(dictItem.Value, typeof(FlowValueType));
        }

        [TestMethod]
        public void ToValueData_NestedStruct_ReturnsList()
        {
            var intType = new Int16Type(123);
            var ufix64Type = new UFix64Type(2345.6m);
            var intField = new CompositeField("intField", intType);
            var ufixField = new CompositeField("ufixField", ufix64Type);

            var composite = new CompositeType("Struct", "structId", new List<CompositeField>() { intField, ufixField });

            //nest the composite inside of the array
            var arrayType = new ArrayType(new List<FlowValueType>() { composite });

            var result = arrayType.ToValueData();
            Assert.IsInstanceOfType(result, typeof(List<object>));

            var list = result as List<object>;
            Assert.AreEqual(1, list.Count);

            var item = list.First();
            Assert.IsInstanceOfType(item, typeof(Dictionary<string, object>)); //this should be a dictionary, ie its the struct's fields

            //verify the struct fields are in this dictionary
            var dict = item as Dictionary<string, object>;
            Assert.AreEqual(2, dict.Keys.Count);

            var first = dict.First();
            Assert.AreEqual("intField", first.Key);
            Assert.AreEqual(intType.Data, first.Value);

            var second = dict.Skip(1).First();
            Assert.AreEqual("ufixField", second.Key);
            Assert.AreEqual(ufix64Type.Data, second.Value);
        }
    }
}