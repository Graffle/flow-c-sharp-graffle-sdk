using System.Collections.Generic;
using System.Linq;
using Graffle.FlowSdk.Services.Extensions;
using Graffle.FlowSdk.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graffle.FlowSdk.Services.Tests.ExtensionTests
{
    [TestClass]
    public class FlowValueTypeUtilityTests
    {
        [TestMethod]
        public void FlowTypeToPrimitive_PreserveDictionaryKeyCasingTrue_PreservesDictionaryKeyCasing()
        {
            const string key = "NFTKey";
            var stringType = new StringType(key);
            var intType = new IntType(5);

            var dictionaryType = new DictionaryType();
            dictionaryType.Data.Add(stringType, intType);

            var res = FlowValueTypeUtility.FlowTypeToPrimitive(dictionaryType, true);

            var dict = res as Dictionary<object, object>;

            var dictKey = dict.Keys.First().ToString();

            Assert.AreEqual(key, dictKey);
        }

        [TestMethod]
        public void FlowTypeToPrimitive_PreserveDictionaryKeyCasingFalse_DoesNotPreservesDictionaryKeyCasing()
        {
            const string key = "NFTKey";
            var stringType = new StringType(key);
            var intType = new IntType(5);

            var dictionaryType = new DictionaryType();
            dictionaryType.Data.Add(stringType, intType);

            var res = FlowValueTypeUtility.FlowTypeToPrimitive(dictionaryType); //default false

            var dict = res as Dictionary<object, object>;

            var dictKey = dict.Keys.First().ToString();

            Assert.AreEqual(key.ToCamelCase(), dictKey); //key should be camel cased with extension method
        }
    }
}