using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Test
{
    [TestClass]
    public class ConvertorTest
    {
        [TestMethod]
        public void ToStringReturnsNullForNull()
        {
            const string defaultValue = "defaultstring";

            var result = Convertor.ToString(null, defaultValue);

            Assert.AreEqual(defaultValue, result);
        }

        [TestMethod]
        public void ToStringReturnsNullForDbNullValue()
        {
            const string defaultValue = "defaultstring";

            var result = Convertor.ToString(DBNull.Value, defaultValue);

            Assert.AreEqual(defaultValue, result);
        }

        [TestMethod]
        public void ToStringReturnsEmptyString()
        {
            const string defaultValue = "defaultstring";

            var result = Convertor.ToString("", defaultValue);

            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void ToStringReturnsStringItself()
        {
            const string stringValue = "mystring";
            const string defaultValue = "defaultstring";

            var result = Convertor.ToString(stringValue, defaultValue);

            Assert.AreEqual(stringValue, result);
        }
    }
}
