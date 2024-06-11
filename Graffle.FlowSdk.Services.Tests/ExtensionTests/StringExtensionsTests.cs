using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Graffle.FlowSdk.Services.Tests.ExtensionTests
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        [DataRow("", "")]
        [DataRow("A", "a")]
        [DataRow("HelloWorld", "helloWorld")]
        [DataRow("helloWorld", "helloWorld")]
        [DataRow("aaaaabaaa", "aaaaabaaa")]
        [DataRow("AaaaaaaBBBBBBB", "aaaaaaaBBBBBBB")]
        [DataRow("CamelCase", "camelCase")]
        public void ToCamelCaseTest(string value, string expected)
        {
            Assert.AreEqual(expected, value.ToCamelCase());
        }
    }
}