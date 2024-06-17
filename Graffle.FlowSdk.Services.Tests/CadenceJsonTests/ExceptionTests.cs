using Graffle.FlowSdk.Services.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Graffle.FlowSdk.Services.Tests.CadenceJsonTests
{
    [TestClass]
    public class ExceptionTests
    {
        [TestMethod]
        public void NullTypes_NoThrow()
        {
            var ex = new CadenceJsonCastException("foo")
            {
                ExpectedType = null,
                ActualType = null
            };

            var msg = ex.ToString();
            Assert.IsNotNull(msg);
            Assert.IsTrue(msg.Length > 0);
        }

        [TestMethod]
        public void ToStringTest()
        {
            var expected = typeof(IDictionary<string, object>);
            var actual = typeof(int);
            var exMsg = "foo";

            var ex = new CadenceJsonCastException(exMsg)
            {
                ExpectedType = expected,
                ActualType = actual
            };

            var msg = ex.ToString();
            Assert.AreEqual(string.Format("{0}, Expected Type {1}, Actual Type {2}", exMsg, expected.ToString(), actual.ToString()), msg);
        }
    }
}