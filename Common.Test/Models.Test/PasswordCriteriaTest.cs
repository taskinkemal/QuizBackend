using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.TransferObjects;

namespace Models.Test
{
    [TestClass]
    public class PasswordCriteriaTest
    {
        [TestMethod]
        public void PasswordCriteriaLengthFalse()
        {
            const string password = "test1";

            var result = PasswordCriteria.IsValid(password);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PasswordCriteriaTrue()
        {
            const string password = "test11";

            var result = PasswordCriteria.IsValid(password);

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void PasswordCriteriaLetterFalse()
        {
            const string password = "123456";

            var result = PasswordCriteria.IsValid(password);

            Assert.IsFalse(result);
        }


        [TestMethod]
        public void PasswordCriteriaNumericFalse()
        {
            const string password = "testtest";

            var result = PasswordCriteria.IsValid(password);

            Assert.IsFalse(result);
        }
    }
}
