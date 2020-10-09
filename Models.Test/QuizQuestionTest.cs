using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;

namespace Models.Test
{
    [TestClass]
    public class QuizQuestionTest
    {
        [TestMethod]
        public void QuizQuestionConstructor()
        {
            var sut = new QuizQuestion
            {
                QuestionId = 23,
                QuizId = 55,
                QuestionOrder = 2
            };

            Assert.AreEqual(23, sut.QuestionId);
            Assert.AreEqual(55, sut.QuizId);
            Assert.AreEqual(2, sut.QuestionOrder);
        }
    }
}
