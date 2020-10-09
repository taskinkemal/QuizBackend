using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;

namespace Models.Test
{
    [TestClass]
    public class DbModelAnswer
    {
        [TestMethod]
        public void AnswerConstructor()
        {
            var answer = new Answer
            {
                AttemptId = 5,
                OptionId = 6,
                QuestionId = 7
            };

            Assert.AreEqual(5, answer.AttemptId);
            Assert.AreEqual(6, answer.OptionId);
            Assert.AreEqual(7, answer.QuestionId);
        }
    }
}
