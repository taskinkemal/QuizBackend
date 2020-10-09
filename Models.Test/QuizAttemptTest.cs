using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;

namespace Models.Test
{
    [TestClass]
    public class QuizAttemptTest
    {
        [TestMethod]
        public void QuizAttemptConstructor()
        {
            var attempt = new QuizAttempt
            {
                Id = 8,
                UserId = 23,
                QuizId = 55,
                Status = QuizAttemptStatus.Passed,
                StartDate = new DateTime(2020, 3, 4),
                EndDate = new DateTime(2020, 4, 4),
                TimeSpent = 88,
                Correct = 4,
                Incorrect = 5,
                Score = 90.5M
            };

            Assert.AreEqual(8, attempt.Id);
            Assert.AreEqual(23, attempt.UserId);
            Assert.AreEqual(55, attempt.QuizId);
            Assert.AreEqual(QuizAttemptStatus.Passed, attempt.Status);
            Assert.AreEqual(new DateTime(2020, 3, 4), attempt.StartDate);
            Assert.AreEqual(new DateTime(2020, 4, 4), attempt.EndDate);
            Assert.AreEqual(88, attempt.TimeSpent);
            Assert.AreEqual(4, attempt.Correct);
            Assert.AreEqual(5, attempt.Incorrect);
            Assert.AreEqual(90.5M, attempt.Score);
        }
    }
}
