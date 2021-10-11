using BusinessLayer.Context;
using BusinessLayer.Implementations;
using BusinessLayer.Interfaces;
using Common.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;
using Models.TransferObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        [TestMethod]
        public async Task GetQuizQuestionsQuizNotFound()
        {
            List<Question> result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuestionManager(context, logManager);

                result = await sut.GetQuizQuestions(5, 8);
            }

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetQuizQuestionsNotOwner()
        {
            List<Question> result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuestionManager(context, logManager);

                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40,
                    AvailableTo = DateTime.Now.AddDays(1)
                };

                var testData = await ManagerTestHelper.CreateAndAssignQuizAsync(context, quiz, true);

                result = await sut.GetQuizQuestions(testData.UserId + 1, testData.QuizId);
            }

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetQuizQuestionsNotOwnerButAssigned()
        {
            List<Question> result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuestionManager(context, logManager);

                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40,
                    AvailableTo = DateTime.Now.AddDays(1)
                };

                var testData = await ManagerTestHelper.CreateAndAssignQuizAsync(context, quiz, false);
                var questionId = await sut.InsertQuestionInternalAsync(ManagerTestHelper.CreateQuestion(0));
                await sut.AssignQuestionInternalAsync(quiz.Id, questionId);
                questionId = await sut.InsertQuestionInternalAsync(ManagerTestHelper.CreateQuestion(1));
                await sut.AssignQuestionInternalAsync(quiz.Id, questionId);
                
                var newAssignedUser = await ManagerTestHelper.AssignQuizAsync(context, quiz.QuizIdentityId);

                result = await sut.GetQuizQuestions(newAssignedUser, testData.QuizId);
            }

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public async Task GetQuizQuestionsNotOwnerNotAssigned()
        {
            List<Question> result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuestionManager(context, logManager);
                var userManager = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>(), logManager);

                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40,
                    AvailableTo = DateTime.Now.AddDays(1)
                };

                var testData = await ManagerTestHelper.CreateAndAssignQuizAsync(context, quiz, false);

                var newAssignedUser = await ManagerTestHelper.AssignQuizAsync(context, quiz.QuizIdentityId);
                var unassignedUserId = await userManager.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(1), true);

                result = await sut.GetQuizQuestions(unassignedUserId, testData.QuizId);
            }

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetQuestionOptions()
        {
            int questionCount;
            List<Option> result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuestionManager(context, logManager);

                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40,
                    AvailableTo = DateTime.Now.AddDays(1)
                };

                var testData = await ManagerTestHelper.CreateQuizAsync(context, 3, 8);
                var userId = await ManagerTestHelper.AssignQuizAsync(context, testData.QuizIdentityId);

                questionCount = testData.QuestionIds.Count;
                result = await sut.GetQuestionOptions(userId, testData.QuizId, testData.QuestionIds[2]);
            }

            Assert.AreEqual(3, questionCount);
            Assert.AreEqual(8, result.Count);
        }

        [TestMethod]
        public async Task GetQuestionOptionsQuizNotFound()
        {
            int questionCount;
            List<Option> result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuestionManager(context, logManager);

                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40,
                    AvailableTo = DateTime.Now.AddDays(1)
                };

                var testData = await ManagerTestHelper.CreateQuizAsync(context, 3, 8);
                var user = await ManagerTestHelper.AddUserAsync(context, 5);

                questionCount = testData.QuestionIds.Count;
                result = await sut.GetQuestionOptions(user.Id, 56, 32);
            }

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetQuestionOptionsUserNotFound()
        {
            int questionCount;
            List<Option> result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuestionManager(context, logManager);

                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40,
                    AvailableTo = DateTime.Now.AddDays(1)
                };

                var testData = await ManagerTestHelper.CreateQuizAsync(context, 3, 8);
                var userId = await ManagerTestHelper.AssignQuizAsync(context, testData.QuizIdentityId);

                questionCount = testData.QuestionIds.Count;
                result = await sut.GetQuestionOptions(Math.Max(testData.OwnerId, userId) + 1, testData.QuizId, testData.QuestionIds[2]);
            }

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetQuestionOptionsNotAssigned()
        {
            int questionCount;
            List<Option> result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuestionManager(context, logManager);
                var userManager = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>(), logManager);
                var unassignedUserId = await userManager.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(1), true);

                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40,
                    AvailableTo = DateTime.Now.AddDays(1)
                };

                var testData = await ManagerTestHelper.CreateQuizAsync(context, 3, 8);
                var userId = await ManagerTestHelper.AssignQuizAsync(context, testData.QuizIdentityId);

                questionCount = testData.QuestionIds.Count;
                result = await sut.GetQuestionOptions(unassignedUserId, testData.QuizId, testData.QuestionIds[2]);
            }

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetQuestionOptionsQuestionNotFound()
        {
            int questionCount;
            List<Option> result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuestionManager(context, logManager);

                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40,
                    AvailableTo = DateTime.Now.AddDays(1)
                };

                var testData = await ManagerTestHelper.CreateQuizAsync(context, 3, 8);
                var userId = await ManagerTestHelper.AssignQuizAsync(context, testData.QuizIdentityId);

                questionCount = testData.QuestionIds.Count;
                result = await sut.GetQuestionOptions(userId, testData.QuizId, testData.QuestionIds.Max() + 1);
            }

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateQuestionQuestionNotFound()
        {
            SaveQuizResult result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuestionManager(context, logManager);

                result = await sut.UpdateQuestion(1, 2, 3, null);
            }

            Assert.AreEqual(SaveQuizResultStatus.GeneralError, result.Status);
        }

        [TestMethod]
        public async Task UpdateQuestionQuestionIdDoesntMatch()
        {
            SaveQuizResult result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuestionManager(context, logManager);

                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40,
                    AvailableTo = DateTime.Now.AddDays(1)
                };

                var testData = await ManagerTestHelper.CreateQuizAsync(context, 3, 8);
                var userId = await ManagerTestHelper.AssignQuizAsync(context, testData.QuizIdentityId);

                var question = (await sut.GetQuizQuestions(userId, testData.QuizId)).FirstOrDefault();

                result = await sut.UpdateQuestion(1, 2, question.Id + 1, question);
            }

            Assert.AreEqual(SaveQuizResultStatus.NotAuthorized, result.Status);
        }

        [TestMethod]
        public async Task UpdateQuestionUserNotAuthorized()
        {
            SaveQuizResult result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuestionManager(context, logManager);

                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40,
                    AvailableTo = DateTime.Now.AddDays(1)
                };

                var testData = await ManagerTestHelper.CreateQuizAsync(context, 3, 8);
                var userId = await ManagerTestHelper.AssignQuizAsync(context, testData.QuizIdentityId);

                var question = (await sut.GetQuizQuestions(userId, testData.QuizId)).FirstOrDefault();

                result = await sut.UpdateQuestion(11, testData.QuizId, question.Id, question);
            }

            Assert.AreEqual(SaveQuizResultStatus.NotAuthorized, result.Status);
        }

        [TestMethod]
        public async Task UpdateQuestionQuestionIsInvalid()
        {
            SaveQuizResult result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuestionManager(context, logManager);

                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40,
                    AvailableTo = DateTime.Now.AddDays(1)
                };

                var testData = await ManagerTestHelper.CreateQuizAsync(context, 3, 8);
                var userId = await ManagerTestHelper.AssignQuizAsync(context, testData.QuizIdentityId);

                var question = (await sut.GetQuizQuestions(userId, testData.QuizId)).FirstOrDefault();
                question.Body = "";

                result = await sut.UpdateQuestion(testData.OwnerId, testData.QuizId, question.Id, question);
            }

            Assert.AreEqual(SaveQuizResultStatus.GeneralError, result.Status);
        }

        [TestMethod]
        public async Task UpdateQuestion()
        {
            SaveQuizResult result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuestionManager(context, logManager);

                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40,
                    AvailableTo = DateTime.Now.AddDays(1)
                };

                var testData = await ManagerTestHelper.CreateQuizAsync(context, 3, 8);
                var userId = await ManagerTestHelper.AssignQuizAsync(context, testData.QuizIdentityId);

                var question = (await sut.GetQuizQuestions(userId, testData.QuizId)).FirstOrDefault();
                question.Body = "new text";

                result = await sut.UpdateQuestion(testData.OwnerId, testData.QuizId, question.Id, question);
            }

            Assert.AreEqual(SaveQuizResultStatus.Success, result.Status);
        }

        [TestMethod]
        public async Task AuthorizeQuestionUpdateRequestQuizNotFound()
        {
            SaveQuizResultStatus result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuestionManager(context, logManager);

                result = await sut.AuthorizeQuestionUpdateRequest(5, 11, 53);
            }

            Assert.AreEqual(SaveQuizResultStatus.GeneralError, result);
        }

        [TestMethod]
        public async Task AuthorizeQuestionUpdateRequestQuestionNotFound()
        {
            SaveQuizResultStatus result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuestionManager(context, logManager);

                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40,
                    AvailableTo = DateTime.Now.AddDays(1)
                };

                var testData = await ManagerTestHelper.CreateQuizAsync(context, 3, 8);

                result = await sut.AuthorizeQuestionUpdateRequest(testData.OwnerId, testData.QuizId, testData.QuestionIds.Max() + 1);
            }

            Assert.AreEqual(SaveQuizResultStatus.GeneralError, result);
        }

        [TestMethod]
        public async Task AuthorizeQuestionUpdateRequestNotOwner()
        {
            SaveQuizResultStatus result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuestionManager(context, logManager);

                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40,
                    AvailableTo = DateTime.Now.AddDays(1)
                };

                var testData = await ManagerTestHelper.CreateQuizAsync(context, 3, 8);

                result = await sut.AuthorizeQuestionUpdateRequest(testData.OwnerId + 1, testData.QuizId, testData.QuestionIds.First());
            }

            Assert.AreEqual(SaveQuizResultStatus.NotAuthorized, result);
        }
    }
}
