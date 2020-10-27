using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.Context;
using BusinessLayer.Implementations;
using BusinessLayer.Interfaces;
using Common.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;
using Models.TransferObjects;
using Moq;

namespace BusinessLayer.Test
{
    [TestClass]
    public class QuizManagerTest
    {
        [TestMethod]
        public async Task GetUserQuizList()
        {
            List<Quiz> result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuizManager(context, new QuizAttemptManager(context, Mock.Of<IQuestionManager>(), logManager), logManager);
                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40,
                    AvailableTo = DateTime.Now.AddDays(1)
                };

                var testData = await ManagerTestHelper.CreateAndAssignQuizAsync(context, quiz, true);
                var attempt = await context.QuizAttempts.AddAsync(
                    new QuizAttempt
                    {
                        QuizId = testData.QuizId,
                        UserId = testData.UserId,
                        Status = QuizAttemptStatus.Incomplete,
                        StartDate = DateTime.Now
                    });
                await context.SaveChangesAsync();

                result = await sut.GetUserQuizList(testData.UserId);
            }

            Assert.IsNotNull(result[0].LastAttempt);
        }

        [TestMethod]
        public async Task GetUserQuizListFinish()
        {
            List<Quiz> result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuizManager(context, new QuizAttemptManager(context, new QuestionManager(context, logManager), logManager), logManager);
                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40,
                    AvailableTo = DateTime.Now.AddDays(-1)
                };

                var testData = await ManagerTestHelper.CreateAndAssignQuizAsync(context, quiz, true);
                var attempt = await context.QuizAttempts.AddAsync(
                    new QuizAttempt
                    {
                        QuizId = testData.QuizId,
                        UserId = testData.UserId,
                        Status = QuizAttemptStatus.Incomplete,
                        StartDate = DateTime.Now.AddDays(-2)
                    });
                await context.SaveChangesAsync();

                result = await sut.GetUserQuizList(testData.UserId);
            }

            Assert.IsNotNull(result[0].LastAttempt);
            Assert.AreEqual(QuizAttemptStatus.Completed, result[0].LastAttempt.Status);
        }

        [TestMethod]
        public async Task GetAdminQuizList()
        {
            List<Quiz> result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var userManager = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>(), logManager);
                var sut = new QuizManager(context, new QuizAttemptManager(context, Mock.Of<IQuestionManager>(), logManager), logManager);
                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40,
                    AvailableTo = DateTime.Now.AddDays(1)
                };

                var userId = await userManager.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(0), true);
                await sut.InsertQuizInternalAsync(userId, ManagerTestHelper.CreateQuiz(0));
                await sut.InsertQuizInternalAsync(userId, ManagerTestHelper.CreateQuiz(1));

                await context.SaveChangesAsync();

                result = await sut.GetAdminQuizList(userId);
            }

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public async Task InsertQuizNull()
        {
            const int userId = 54;
            SaveQuizResult result = null;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = new QuizManager(context, Mock.Of<IQuizAttemptManager>(), Mock.Of<ILogManager>());

                result = await sut.InsertQuiz(userId, null);
            }

            Assert.AreEqual(SaveQuizResultStatus.GeneralError, result.Status);
        }

        [TestMethod]
        public async Task InsertQuizIdNotDefault()
        {
            const int userId = 54;
            SaveQuizResult result = null;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = new QuizManager(context, Mock.Of<IQuizAttemptManager>(), Mock.Of<ILogManager>());

                result = await sut.InsertQuiz(userId, new Quiz
                {
                    Id = 5
                });
            }

            Assert.AreEqual(SaveQuizResultStatus.NotAuthorized, result.Status);
        }

        [TestMethod]
        public async Task InsertQuizIdentityIdNotDefault()
        {
            const int userId = 54;
            SaveQuizResult result = null;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = new QuizManager(context, Mock.Of<IQuizAttemptManager>(), Mock.Of<ILogManager>());

                result = await sut.InsertQuiz(userId, new Quiz
                {
                    QuizIdentityId = 5
                });
            }

            Assert.AreEqual(SaveQuizResultStatus.NotAuthorized, result.Status);
        }

        [TestMethod]
        public async Task InsertQuizNotValid()
        {
            const int userId = 54;
            SaveQuizResult result = null;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = new QuizManager(context, Mock.Of<IQuizAttemptManager>(), Mock.Of<ILogManager>());

                result = await sut.InsertQuiz(userId, new Quiz
                {
                    PassScore = -10
                });
            }

            Assert.AreEqual(SaveQuizResultStatus.GeneralError, result.Status);
        }

        [TestMethod]
        public async Task InsertQuizSuccess()
        {
            const int userId = 54;
            const string title = "Quiz Title";
            const string intro = "Quiz Intro";
            const int passScore = 75;

            SaveQuizResult result = null;
            Quiz addedQuiz = null;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = new QuizManager(context, Mock.Of<IQuizAttemptManager>(), Mock.Of<ILogManager>());

                result = await sut.InsertQuiz(userId, new Quiz
                {
                    Title = title,
                    Intro = intro,
                    PassScore = passScore
                });

                addedQuiz = await context.Quizes.FindAsync(result.Result);
            }

            Assert.AreEqual(SaveQuizResultStatus.Success, result.Status);
            Assert.AreEqual(title, addedQuiz.Title);
            Assert.AreEqual(intro, addedQuiz.Intro);
            Assert.AreEqual(passScore, addedQuiz.PassScore);
            Assert.AreEqual(1, addedQuiz.Version);
            Assert.AreEqual(QuizStatus.Current, addedQuiz.Status);
        }

        [TestMethod]
        public async Task UpdateQuizNull()
        {
            const int userId = 54;
            const int quizId = 134;
            SaveQuizResult result = null;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = new QuizManager(context, Mock.Of<IQuizAttemptManager>(), Mock.Of<ILogManager>());

                result = await sut.UpdateQuiz(userId, quizId, null);
            }

            Assert.AreEqual(SaveQuizResultStatus.GeneralError, result.Status);
        }

        [TestMethod]
        public async Task UpdateQuizIdNotMatching()
        {
            const int userId = 54;
            const int quizId = 134;
            SaveQuizResult result = null;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = new QuizManager(context, Mock.Of<IQuizAttemptManager>(), Mock.Of<ILogManager>());

                result = await sut.UpdateQuiz(userId, quizId, new Quiz
                {
                    Id = quizId + 1
                });
            }

            Assert.AreEqual(SaveQuizResultStatus.NotAuthorized, result.Status);
        }

        [TestMethod]
        public async Task UpdateQuizIdNotAuthorized()
        {
            const int userId = 54;
            const int quizId = 134;
            SaveQuizResult result = null;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = new QuizManager(context, Mock.Of<IQuizAttemptManager>(), Mock.Of<ILogManager>());

                result = await sut.UpdateQuiz(userId, quizId, new Quiz
                {
                    Id = quizId
                });
            }

            Assert.AreEqual(SaveQuizResultStatus.NotAuthorized, result.Status);
        }

        [TestMethod]
        public async Task UpdateQuizNotValid()
        {
            const int userId = 54;
            SaveQuizResult result = null;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = new QuizManager(context, Mock.Of<IQuizAttemptManager>(), Mock.Of<ILogManager>());

                var insertResult = await sut.InsertQuizInternalAsync(userId, new Quiz
                {
                    Title = "",
                    Intro = "",
                    Version = 1,
                    Status = QuizStatus.Current
                });

                result = await sut.UpdateQuiz(userId, insertResult.QuizId, new Quiz
                {
                    Id = insertResult.QuizId,
                    QuizIdentityId = insertResult.QuizIdentityId,
                    Version = 1,
                    Status = QuizStatus.Current,
                    PassScore = -10
                });
            }

            Assert.AreEqual(SaveQuizResultStatus.GeneralError, result.Status);
        }

        [TestMethod]
        public async Task UpdateQuizSuccess()
        {
            const int userId = 54;
            const string title = "Quiz Title";
            const string intro = "Quiz Intro";
            const int passScore = 75;
            SaveQuizResult result = null;
            Quiz updatedQuiz = null;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = new QuizManager(context, Mock.Of<IQuizAttemptManager>(), Mock.Of<ILogManager>());

                var insertResult = await sut.InsertQuizInternalAsync(userId, new Quiz
                {
                    Title = "",
                    Intro = "",
                    Version = 1,
                    Status = QuizStatus.Current,
                    PassScore = 10
                });

                var quiz = await context.Quizes.FindAsync(insertResult.QuizId);

                quiz.Title = title;
                quiz.Intro = intro;
                quiz.PassScore = passScore;

                result = await sut.UpdateQuiz(userId, insertResult.QuizId, quiz);

                updatedQuiz = await context.Quizes.FindAsync(insertResult.QuizId);
            }

            Assert.AreEqual(SaveQuizResultStatus.Success, result.Status);
            Assert.AreEqual(title, updatedQuiz.Title);
            Assert.AreEqual(intro, updatedQuiz.Intro);
            Assert.AreEqual(passScore, updatedQuiz.PassScore);
            Assert.AreEqual(1, updatedQuiz.Version);
            Assert.AreEqual(QuizStatus.Current, updatedQuiz.Status);
        }

        [TestMethod]
        public async Task AuthorizeQuizUpdateRequestNull()
        {
            const int userId = 54;
            SaveQuizResultStatus result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = new QuizManager(context, Mock.Of<IQuizAttemptManager>(), Mock.Of<ILogManager>());

                result = await sut.AuthorizeQuizUpdateRequest(userId, null);
            }

            Assert.AreEqual(SaveQuizResultStatus.GeneralError, result);
        }

        [TestMethod]
        public async Task AuthorizeQuizUpdateRequestDbNull()
        {
            const int userId = 54;
            SaveQuizResultStatus result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = new QuizManager(context, Mock.Of<IQuizAttemptManager>(), Mock.Of<ILogManager>());

                result = await sut.AuthorizeQuizUpdateRequest(userId, new Quiz
                {
                    Id = 123,
                    QuizIdentityId = 444,
                    Title = "",
                    Intro = "",
                    Version = 1,
                    Status = QuizStatus.Current,
                    PassScore = 10
                });
            }

            Assert.AreEqual(SaveQuizResultStatus.NotAuthorized, result);
        }

        [TestMethod]
        public async Task AuthorizeQuizUpdateRequestIdentityIdNotMatching()
        {
            const int userId = 54;
            SaveQuizResultStatus result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = new QuizManager(context, Mock.Of<IQuizAttemptManager>(), Mock.Of<ILogManager>());

                var insertResult = await sut.InsertQuizInternalAsync(userId, new Quiz
                {
                    Title = "",
                    Intro = "",
                    Version = 1,
                    Status = QuizStatus.Current,
                    PassScore = 10
                });

                result = await sut.AuthorizeQuizUpdateRequest(userId, new Quiz
                {
                    Id = insertResult.QuizId,
                    QuizIdentityId = insertResult.QuizIdentityId + 1,
                    Title = "",
                    Intro = "",
                    Version = 1,
                    Status = QuizStatus.Current,
                    PassScore = 10
                });
            }

            Assert.AreEqual(SaveQuizResultStatus.NotAuthorized, result);
        }

        [TestMethod]
        public async Task AuthorizeQuizUpdateRequestVersionNotMatching()
        {
            const int userId = 54;
            SaveQuizResultStatus result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = new QuizManager(context, Mock.Of<IQuizAttemptManager>(), Mock.Of<ILogManager>());

                var insertResult = await sut.InsertQuizInternalAsync(userId, new Quiz
                {
                    Title = "",
                    Intro = "",
                    Version = 1,
                    Status = QuizStatus.Current,
                    PassScore = 10
                });

                result = await sut.AuthorizeQuizUpdateRequest(userId, new Quiz
                {
                    Id = insertResult.QuizId,
                    QuizIdentityId = insertResult.QuizIdentityId,
                    Title = "",
                    Intro = "",
                    Version = 2,
                    Status = QuizStatus.Current,
                    PassScore = 10
                });
            }

            Assert.AreEqual(SaveQuizResultStatus.GeneralError, result);
        }

        [TestMethod]
        public async Task AuthorizeQuizUpdateRequestStatusNotMatching()
        {
            const int userId = 54;
            SaveQuizResultStatus result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = new QuizManager(context, Mock.Of<IQuizAttemptManager>(), Mock.Of<ILogManager>());

                var insertResult = await sut.InsertQuizInternalAsync(userId, new Quiz
                {
                    Title = "",
                    Intro = "",
                    Version = 1,
                    Status = QuizStatus.Current,
                    PassScore = 10
                });

                result = await sut.AuthorizeQuizUpdateRequest(userId, new Quiz
                {
                    Id = insertResult.QuizId,
                    QuizIdentityId = insertResult.QuizIdentityId,
                    Title = "",
                    Intro = "",
                    Version = 1,
                    Status = QuizStatus.Draft,
                    PassScore = 10
                });
            }

            Assert.AreEqual(SaveQuizResultStatus.GeneralError, result);
        }

        [TestMethod]
        public async Task AuthorizeQuizUpdateRequestOwnerNotMatching()
        {
            const int userId = 54;
            SaveQuizResultStatus result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = new QuizManager(context, Mock.Of<IQuizAttemptManager>(), Mock.Of<ILogManager>());

                var insertResult = await sut.InsertQuizInternalAsync(userId, new Quiz
                {
                    Title = "",
                    Intro = "",
                    Version = 1,
                    Status = QuizStatus.Current,
                    PassScore = 10
                });

                result = await sut.AuthorizeQuizUpdateRequest(userId + 1, new Quiz
                {
                    Id = insertResult.QuizId,
                    QuizIdentityId = insertResult.QuizIdentityId,
                    Title = "",
                    Intro = "",
                    Version = 1,
                    Status = QuizStatus.Current,
                    PassScore = 10
                });
            }

            Assert.AreEqual(SaveQuizResultStatus.NotAuthorized, result);
        }

        [TestMethod]
        public async Task AuthorizeQuizUpdateRequestSuccess()
        {
            const int userId = 54;
            SaveQuizResultStatus result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = new QuizManager(context, Mock.Of<IQuizAttemptManager>(), Mock.Of<ILogManager>());

                var insertResult = await sut.InsertQuizInternalAsync(userId, new Quiz
                {
                    Title = "",
                    Intro = "",
                    Version = 1,
                    Status = QuizStatus.Current,
                    PassScore = 10
                });

                result = await sut.AuthorizeQuizUpdateRequest(userId, new Quiz
                {
                    Id = insertResult.QuizId,
                    QuizIdentityId = insertResult.QuizIdentityId,
                    Title = "",
                    Intro = "",
                    Version = 1,
                    Status = QuizStatus.Current,
                    PassScore = 10
                });
            }

            Assert.AreEqual(SaveQuizResultStatus.Success, result);
        }

        [TestMethod]
        public void IsValidNull()
        {
            var actual = QuizManager.IsValid(null);

            Assert.IsFalse(actual);
        }

        [DataTestMethod]
        [DataRow(true, false, null, 20, null, null)]
        [DataRow(true, false, null, 20, 2020, null)]
        [DataRow(true, false, null, 20, null, 2021)]
        [DataRow(true, false, null, 20, 2019, 2021)]
        [DataRow(false, false, null, 20, 2021, 2020)]
        [DataRow(false, true, null, 20, null, null)]
        [DataRow(false, true, -10, 20, null, null)]
        [DataRow(false, true, 0, 20, null, null)]
        [DataRow(true, true, 10, 0, null, null)]
        [DataRow(false, false, 0, 110, null, null)]
        [DataRow(false, false, 0, -10, null, null)]
        [DataRow(true, false, 0, null, null, null)]
        public void IsValid(bool expected, bool timeConstraint, int timeLimitInSeconds, int? passScore, int? availableFromYear, int? availableToYear)
        {
            var actual = QuizManager.IsValid(new Quiz
            {
                TimeConstraint = timeConstraint,
                TimeLimitInSeconds = timeLimitInSeconds,
                PassScore = passScore,
                AvailableFrom = availableFromYear.HasValue ? new DateTime(availableFromYear.Value, 1, 1) : default(DateTime?),
                AvailableTo = availableToYear.HasValue ? new DateTime(availableToYear.Value, 1, 1) : default(DateTime?)
            });

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task IsOwnerNull()
        {
            const int userId = 54;
            bool result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = new QuizManager(context, Mock.Of<IQuizAttemptManager>(), Mock.Of<ILogManager>());

                result = await sut.IsOwner(userId, 522);
            }

            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow(true, 54, 54)]
        [DataRow(false, 65, 12)]
        public async Task IsOwner(bool expected, int userId, int ownerId)
        {
            bool result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = new QuizManager(context, Mock.Of<IQuizAttemptManager>(), Mock.Of<ILogManager>());

                var insertResult = await sut.InsertQuizInternalAsync(ownerId, ManagerTestHelper.CreateQuiz(0));
                result = await sut.IsOwner(userId, insertResult.QuizIdentityId);
            }

            Assert.AreEqual(expected, result);
        }
    }
}
