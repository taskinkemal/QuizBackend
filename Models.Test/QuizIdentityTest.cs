using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;

namespace Models.Test
{
    [TestClass]
    public class QuizIdentityTest
    {
        [TestMethod]
        public void QuizIdentityConstructor()
        {
            var sut = new QuizIdentity
            {
                OwnerId = 23,
                Id = 55
            };

            Assert.AreEqual(23, sut.OwnerId);
            Assert.AreEqual(55, sut.Id);
        }
    }
}
