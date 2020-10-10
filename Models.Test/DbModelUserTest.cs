using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;

namespace Models.Test
{
    [TestClass]
    public class DbModelUserTest
    {
        [TestMethod]
        public void DbModelUserConstructor()
        {
            var sut = new User
            {
                Id = 74,
                Email = "myemail@email.com"
            };

            Assert.AreEqual(74, sut.Id);
            Assert.AreEqual("myemail@email.com", sut.Email);
        }
    }
}
