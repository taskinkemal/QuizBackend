﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;

namespace Models.Test
{
    [TestClass]
    public class QuizTest
    {
        [DataTestMethod]
        [DataRow(true, null, null)]
        [DataRow(true, -1, null)]
        [DataRow(true, -1, 1)]
        [DataRow(true, null, 1)]
        [DataRow(false, 1, null)]
        [DataRow(false, -2, -1)]
        [DataRow(false, null, -1)]
        [DataRow(true, 0, 1)]
        [DataRow(true, 0, null)]
        [DataRow(false, -1, 0)]
        [DataRow(false, null, 0)]
        public void IsAvailable(bool expected, int? addYearFrom, int? addYearTo)
        {
            var now = DateTime.Now;
            var start = addYearFrom.HasValue ? now.AddYears(addYearFrom.Value) : (DateTime?)null;
            var end = addYearTo.HasValue ? now.AddYears(addYearTo.Value) : (DateTime?)null;
            var quiz = new Quiz
            {
                AvailableFrom = start,
                AvailableTo = end
            };

            Assert.AreEqual(expected, Quiz.IsAvailable(quiz));
        }

        [DataTestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        public void CanStartLastAttemptIsNull(bool expected, bool isRepeatable)
        {
            var quiz = new Quiz
            {
                LastAttempt = null,
                Repeatable = isRepeatable
            };

            Assert.AreEqual(expected, quiz.CanStart);
        }

        [DataTestMethod]
        [DataRow(false, QuizAttemptStatus.Incomplete, true)]
        [DataRow(false, QuizAttemptStatus.Incomplete, false)]
        [DataRow(true, QuizAttemptStatus.Completed, true)]
        [DataRow(false, QuizAttemptStatus.Completed, false)]
        public void CanStartLastAttemptIsNotNull(bool expected, QuizAttemptStatus status, bool isRepeatable)
        {
            var quiz = new Quiz
            {
                LastAttempt = new QuizAttempt
                {
                    Status = status
                },
                Repeatable = isRepeatable
            };

            Assert.AreEqual(expected, quiz.CanStart);
        }
    }
}
