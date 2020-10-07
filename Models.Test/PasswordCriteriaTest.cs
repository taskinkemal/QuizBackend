using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.TransferObjects;

namespace Models.Test
{
    [TestClass]
    public class PasswordCriteriaTest
    {
        [DataTestMethod]
        [DataRow(false, "test1", DisplayName = "LengthNotSatisfied")]
        [DataRow(true, "test11", DisplayName = "Satisfied")]
        [DataRow(false, "123456", DisplayName = "LettersNotSatisfied")]
        [DataRow(false, "testtest", DisplayName = "NumeralsNotSatisfied")]
        [DataRow(false, null, DisplayName = "PasswordIsNull")]
        public void PasswordCriteriaIsValid(bool expected, string password)
        {
            var result = PasswordCriteria.IsValid(password);

            Assert.AreEqual(expected, result);
        }
    }
}
