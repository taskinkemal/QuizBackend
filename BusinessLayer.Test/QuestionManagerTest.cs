using BusinessLayer.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;

namespace BusinessLayer.Test
{
    [TestClass]
    public class QuestionManagerTest
    {
        internal bool IsValid(Question question)
        {
            if (question == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(question.Body))
            {
                return false;
            }

            return true;
        }

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
        [DataRow(false, 0)]
        [DataRow(true, 1)]
        [DataRow(true, 3)]
        [DataRow(true, 5)]
        [DataRow(false, 6)]
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
