using BusinessLayer.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;

namespace BusinessLayer.Test
{
    [TestClass]
    public class QuestionManagerTest
    {
        [TestMethod]
        public void IsValidNull()
        {
            var actual = QuestionManager.IsValid(null);

            Assert.IsFalse(actual);
        }

        [DataTestMethod]
        [DataRow(false, " ")]
        [DataRow(false, null)]
        [DataRow(true, "test")]
        public void IsValidBody(bool expected, string body)
        {
            var actual = QuestionManager.IsValid(new Question
            {
                Body = body,
                Level = 3
            });

            Assert.AreEqual(expected, actual);
        }


        [DataTestMethod]
        [DataRow(false, (byte)0)]
        [DataRow(true, (byte)1)]
        [DataRow(true, (byte)3)]
        [DataRow(true, (byte)5)]
        [DataRow(false, (byte)6)]
        public void IsValidLevel(bool expected, byte level)
        {
            var actual = QuestionManager.IsValid(new Question
            {
                Body = "test",
                Level = level
            });

            Assert.AreEqual(expected, actual);
        }
    }
}
