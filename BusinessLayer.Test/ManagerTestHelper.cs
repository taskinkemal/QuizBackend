using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.Context;
using BusinessLayer.Implementations;
using BusinessLayer.Interfaces;
using Common;
using Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.DbModels;
using Moq;

namespace BusinessLayer.Test
{
    internal static class ManagerTestHelper
    {
        private static int Counter = 0;

        internal static DbContextOptions<QuizContext> Options => new DbContextOptionsBuilder<QuizContext>()
            .UseInMemoryDatabase("TestDatabase" + Counter++)
            .Options;

        internal static AuthManager GetAuthManager(QuizContext context)
        {
            return new AuthManager(context, Mock.Of<ILogManager>());
        }

        internal static UserManager GetUserManager(QuizContext context, IAuthManager authManager, ILogManager logManager = null)
        {
            return new UserManager(context, authManager, logManager ?? Mock.Of<ILogManager>(), Mock.Of<IEmailManager>());
        }

        internal static async Task<User> AddUserAsync(QuizContext context, int testId, string email = "", string password = "", bool isVerified = true)
        {
            var user = await context.Users.AddAsync(new User
            {
                Email = !string.IsNullOrWhiteSpace(email) ? email : "user" + testId + "@mymail.com",
                FirstName = "Name" + testId,
                LastName = "Surname" + testId,
                PasswordHash = AuthenticationHelper.EncryptPassword(!string.IsNullOrWhiteSpace(password) ? password : "otherpassword123_" + testId),
                PictureUrl = "",
                IsVerified = isVerified
            });

            return user.Entity;
        }

        internal static Models.TransferObjects.User CreateUserTo(int testId, string email = "", string password = "")
        {
            return new Models.TransferObjects.User
            {
                Email = !string.IsNullOrWhiteSpace(email) ? email : "user" + testId + "@mymail.com",
                FirstName = "Name" + testId,
                LastName = "Surname" + testId,
                DeviceId = "",
                PictureUrl = "",
                Password = !string.IsNullOrWhiteSpace(password) ? password : "otherpassword123_" + testId
            };
        }

        internal static async Task<UserToken> AddAuthTokenAsync(QuizContext context, int userId, string deviceId, string token, bool isValid)
        {
            var userToken = await context.UserTokens.AddAsync(new UserToken
            {
                UserId = userId,
                DeviceId = deviceId,
                Token = token,
                ValidUntil = DateTime.Now.AddYears(isValid ? 1 : -1)
            });

            return userToken.Entity;
        }

        internal static async Task<OneTimeToken> AddOneTimeTokenAsync(QuizContext context, string email, OneTimeTokenType tokenType, string token, bool isValid)
        {
            var oneTimeToken = await context.OneTimeTokens.AddAsync(new OneTimeToken
            {
                Email = email,
                TokenType = (byte)tokenType,
                Token = token,
                ValidUntil = DateTime.Now.AddYears(isValid ? 1 : -1)
            });

            return oneTimeToken.Entity;
        }

        internal static Quiz CreateQuiz(int testId)
        {
            return new Quiz
            {
                Title = "Quiz Title " + testId,
                Intro = "Quiz Intro " + testId
            };
        }

        internal static Question CreateQuestion(int testId, QuestionType type = QuestionType.MultiSelect, byte level = 3)
        {
            return new Question
            {
                Body = "Question Body " + testId,
                Type = type,
                Level = level
            };
        }

        internal static Option CreateOption(int testId, bool isCorrect)
        {
            return new Option
            {
                Body = "Option Body " + testId,
                IsCorrect = isCorrect
            };
        }

        internal static async Task<(int QuizId, List<int> QuestionIds)> CreateQuizAsync(QuizContext context, int questionCount, int optionCount)
        {
            var logManager = Mock.Of<ILogManager>();
            var optionManager = new OptionManager(context, logManager);
            var quizManager = new QuizManager(context, logManager);
            var questionManager = new QuestionManager(context, logManager);
            var userManager = GetUserManager(context, Mock.Of<IAuthManager>(), logManager);

            var userId = await userManager.InsertUserInternalAsync(CreateUserTo(0), true);
            await quizManager.InsertQuizInternalAsync(userId, CreateQuiz(0));
            var quizId = (await quizManager.InsertQuizInternalAsync(userId, CreateQuiz(1))).QuizId;
            await quizManager.InsertQuizInternalAsync(userId, CreateQuiz(2));

            var questionIds = new List<int>();

            for (var i = 0; i < questionCount; i++)
            {
                var questionId = await questionManager.InsertQuestionInternalAsync(CreateQuestion(i));
                await questionManager.AssignQuestionInternalAsync(quizId, questionId);

                questionIds.Add(questionId);

                for (var j = 0; j < optionCount; j++)
                {
                    var optionId = await optionManager.InsertOptionInternalAsync(CreateOption(j, j == 1));

                    await optionManager.AssignOptionInternalAsync(questionId, optionId);
                }
            }

            return (QuizId: quizId, QuestionIds: questionIds);
        }

        internal static async Task<(int QuizId, int UserId)> CreateAndAssignQuizAsync(QuizContext context, bool assignUser)
        {
            return await CreateAndAssignQuizAsync(context, CreateQuiz(1), assignUser);
        }


        internal static async Task<(int QuizId, int UserId)> CreateAndAssignQuizAsync(QuizContext context, Quiz quiz, bool assignUser)
        {
            var logManager = Mock.Of<ILogManager>();
            var quizManager = new QuizManager(context, logManager);
            var userManager = GetUserManager(context, Mock.Of<IAuthManager>(), logManager);

            var userTo = CreateUserTo(0);
            var userId = await userManager.InsertUserInternalAsync(userTo, true);
            await quizManager.InsertQuizInternalAsync(userId, CreateQuiz(0));
            var quizInsertResult = await quizManager.InsertQuizInternalAsync(userId, quiz);
            await quizManager.InsertQuizInternalAsync(userId, CreateQuiz(2));
            if (assignUser)
            {
                await context.QuizAssignments.AddAsync(
                    new QuizAssignment
                    {
                        QuizIdentityId = quiz.QuizIdentityId,
                        Email = userTo.Email
                    });
            }

            await context.SaveChangesAsync();

            return (QuizId: quizInsertResult.QuizId, UserId: userId);
        }
    }
}
