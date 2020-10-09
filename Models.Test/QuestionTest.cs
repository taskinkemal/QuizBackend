using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;

namespace Models.Test
{
    [TestClass]
    public class QuestionTest
    {
        [TestMethod]
        public void QuestionConstructor()
        {
            var question = new Question
            {
                Id = 4,
                Body = "question body",
                Level = 4,
                Type = QuestionType.MultiSelectMultiOptions,
                OptionIds = new[] { 5, 6 }
            };

            Assert.AreEqual(4, question.Id);
            Assert.AreEqual("question body", question.Body);
            Assert.AreEqual(4, question.Level);
            Assert.AreEqual(QuestionType.MultiSelectMultiOptions, question.Type);
            Assert.IsTrue(question.OptionIds.Contains(5));
            Assert.IsTrue(question.OptionIds.Contains(6));
            Assert.AreEqual(2, question.OptionIds.Count());
        }
    }
}
