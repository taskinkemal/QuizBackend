using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;

namespace Models.Test
{
    [TestClass]
    public class OneTimeTokenTest
    {
        [TestMethod]
        public void OneTimeTokenConstructor()
        {
            var token = new OneTimeToken
            {
                Email = "myemail@email.com",
                Id = 5,
                Token = "tokenstring",
                ValidUntil = new DateTime(2020, 3, 4),
                TokenType = (byte)OneTimeTokenType.AccountVerification
            };

            Assert.AreEqual("myemail@email.com", token.Email);
            Assert.AreEqual(5, token.Id);
            Assert.AreEqual("tokenstring", token.Token);
            Assert.AreEqual(new DateTime(2020, 3, 4), token.ValidUntil);
            Assert.AreEqual(OneTimeTokenType.AccountVerification, (OneTimeTokenType)token.TokenType);
        }
    }
}
