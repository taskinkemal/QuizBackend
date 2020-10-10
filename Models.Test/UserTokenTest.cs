using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;

namespace Models.Test
{
    [TestClass]
    public class UserTokenTest
    {
        [TestMethod]
        public void UserTokenConstructor()
        {
            var sut = new UserToken
            {
                UserId = 74,
                DeviceId = "device id",
                Token = "my token",
                ValidUntil = new System.DateTime(2021, 4, 5)
            };

            Assert.AreEqual(74, sut.UserId);
            Assert.AreEqual("device id", sut.DeviceId);
            Assert.AreEqual("my token", sut.Token);
            Assert.AreEqual(new System.DateTime(2021, 4, 5), sut.ValidUntil);
        }
    }
}
