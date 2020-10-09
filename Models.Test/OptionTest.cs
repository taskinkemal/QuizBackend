using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;

namespace Models.Test
{
    [TestClass]
    public class OptionTest
    {
        [TestMethod]
        public void OptionConstructor()
        {
            var option = new Option
            {
                Id = 6,
                Body = "option body",
                IsCorrect = true
            };

            Assert.AreEqual(6, option.Id);
            Assert.AreEqual("option body", option.Body);
            Assert.AreEqual(true, option.IsCorrect);
        }
    }
}
