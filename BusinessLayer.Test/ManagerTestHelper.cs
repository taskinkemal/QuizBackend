using System;
using System.Threading.Tasks;
using BusinessLayer.Context;
using BusinessLayer.Implementations;
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

        internal static async Task<User> AddUserAsync(QuizContext context, int testId, string email = "", string password = "")
        {
            var user = await context.Users.AddAsync(new User
            {
                Email = !string.IsNullOrWhiteSpace(email) ? email : "user" + testId + "@mymail.com",
                FirstName = "Name" + testId,
                LastName = "Surname" + testId,
                PasswordHash = AuthenticationHelper.EncryptPassword(!string.IsNullOrWhiteSpace(password) ? password : "otherpassword123_" + testId),
                PictureUrl = "",
                IsVerified = true
            });

            return user.Entity;
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
    }
}
