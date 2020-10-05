using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Test
{
    [TestClass]
    public class AuthenticationHelperTest
    {
        [TestMethod]
        public void EncryptPasswordCreatesTheSameHashForTheSamePassword()
        {
            const string password = "mypassword123";

            var hash1 = AuthenticationHelper.EncryptPassword(password);
            var hash2 = AuthenticationHelper.EncryptPassword(password);

            var isSame = AuthenticationHelper.CompareByteArrays(hash1, hash2);

            Assert.IsTrue(isSame);
        }

        [TestMethod]
        public void EncryptPasswordCreatesDifferentHashesForDifferentPasswords()
        {
            const string password1 = "mypassword123";
            const string password2 = "mypassword124";

            var hash1 = AuthenticationHelper.EncryptPassword(password1);
            var hash2 = AuthenticationHelper.EncryptPassword(password2);

            Assert.AreNotEqual(hash1, hash2);
        }

        [TestMethod]
        public void GenerateTokenEndsWithUserId()
        {
            const int userId = 533;

            var token = AuthenticationHelper.GenerateToken(userId);

            Assert.IsTrue(token.EndsWith("_" + userId));
        }

        [TestMethod]
        public void GenerateTokenContainsExactlyOneUnderscore()
        {
            const int userId = 533;

            var token = AuthenticationHelper.GenerateToken(userId);

            Assert.AreEqual(2, token.Split(new char[] { '_' }).ToList().Count);
        }

        [TestMethod]
        public void GenerateRandomStringGeneratesDifferentStrings()
        {
            const int count = 50;

            var list = new List<string>();
            for (var i = 0; i < count; i++)
            {
                list.Add(AuthenticationHelper.GenerateRandomString(160));
            }

            Assert.AreEqual(50, list.Distinct().Count());
        }

        [TestMethod]
        [DataRow(false, new byte[] { 1, 2 }, new byte[] { 1, 2, 3 })]
        [DataRow(false, null, null)]
        [DataRow(false, null, new byte[] { 1, 2, 3 })]
        [DataRow(false, new byte[] { 1, 2, 3 }, null)]
        public void CompareByteArraysNull(bool expected, byte[] arr1, byte[] arr2)
        {
            var actual = AuthenticationHelper.CompareByteArrays(arr1, arr2);

            Assert.AreEqual(expected, actual);
        }
    }
}
