using System.Threading.Tasks;
using BusinessLayer.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;
using Models.TransferObjects;
using User = Models.DbModels.User;

namespace BusinessLayer.Test
{
    [TestClass]
    public class AuthManagerTest
    {
        [TestMethod]
        public async Task GenerateTokenPasswordMatches()
        {
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            AuthToken token;
            User user;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetAuthManager(context);

                await ManagerTestHelper.AddUserAsync(context, 0); 
                user = await ManagerTestHelper.AddUserAsync(context, 1, email, password);
                await ManagerTestHelper.AddUserAsync(context, 2);

                await context.SaveChangesAsync();

                token = await sut.GenerateTokenAsync(new TokenRequest
                {
                    DeviceId = "deviceId",
                    Email = email,
                    Password = password
                });
            }

            Assert.IsNotNull(token);
            Assert.AreEqual(token.UserId, user.Id);
        }

        [TestMethod]
        public async Task GenerateTokenPasswordFail()
        {
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            AuthToken token;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetAuthManager(context);

                var userManager = ManagerTestHelper.GetUserManager(context, sut);
                await userManager.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(0), true);
                await userManager.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(1, email, password), true);
                await userManager.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(2), true);

                await context.SaveChangesAsync();

                token = await sut.GenerateTokenAsync(new TokenRequest
                {
                    DeviceId = "deviceId",
                    Email = email,
                    Password = "differentpass1"
                });
            }

            Assert.IsNull(token);
        }

        [TestMethod]
        public async Task GenerateTokenEmailFail()
        {
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            AuthToken token;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetAuthManager(context);

                var userManager = ManagerTestHelper.GetUserManager(context, sut);
                await userManager.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(0), true);
                await userManager.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(1, email, password), true);
                await userManager.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(2), true);

                await context.SaveChangesAsync();

                token = await sut.GenerateTokenAsync(new TokenRequest
                {
                    DeviceId = "deviceId",
                    Email = "someother@email.com",
                    Password = password
                });
            }

            Assert.IsNull(token);
        }

        [TestMethod]
        public async Task GenerateTokenByIdExistingToken()
        {
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            const string deviceId = "mydevice";
            const string tokenString = "randomtokenstring12345";
            AuthToken token;
            UserToken existingToken;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetAuthManager(context);

                var user = await ManagerTestHelper.AddUserAsync(context, 1, email, password);
                await ManagerTestHelper.AddAuthTokenAsync(context, user.Id, deviceId, tokenString, true);

                await context.SaveChangesAsync();

                token = await sut.GenerateTokenAsync(user.Id, deviceId);

                existingToken = await context.UserTokens.FindAsync(user.Id, deviceId);
            }

            Assert.AreNotEqual(tokenString, existingToken.Token);
            Assert.AreEqual(token.Token, existingToken.Token);
        }

        [TestMethod]
        public async Task GenerateTokenByIdNewToken()
        {
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            const string deviceId = "mydevice";
            AuthToken token;
            UserToken existingToken;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetAuthManager(context);

                var user = await ManagerTestHelper.AddUserAsync(context, 1, email, password);

                await context.SaveChangesAsync();

                token = await sut.GenerateTokenAsync(user.Id, deviceId);

                existingToken = await context.UserTokens.FindAsync(user.Id, deviceId);
            }

            Assert.AreEqual(token.Token, existingToken.Token);
        }

        [TestMethod]
        public async Task GenerateTokenByIdNoUser()
        {
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            const string deviceId = "mydevice";
            AuthToken token;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetAuthManager(context);

                var user = await ManagerTestHelper.AddUserAsync(context, 1, email, password);

                await context.SaveChangesAsync();

                token = await sut.GenerateTokenAsync(user.Id + 1, deviceId); // +1 to simply specify a different user.
            }

            Assert.IsNull(token);
        }

        [TestMethod]
        public async Task GetAccessTokenReturnsNull()
        {
            UserToken token;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetAuthManager(context);

                token = await sut.GetAccessToken("somestring");
            }

            Assert.IsNull(token);
        }

        [TestMethod]
        public async Task GetAccessTokenReturnsToken()
        {
            var token = await GetAccessTokenInternal(true);

            Assert.IsNotNull(token);
        }

        [TestMethod]
        public async Task GetAccessTokenReturnsNullIfNotValid()
        {
            var token = await GetAccessTokenInternal(false);

            Assert.IsNull(token);
        }

        [TestMethod]
        public async Task VerifyAccessToken()
        {
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            const string deviceId = "mydevice";
            const string tokenString = "randomtokenstring12345";
            const bool isUserVerified = false;
            AuthToken token;
            User user;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetAuthManager(context);

                user = await ManagerTestHelper.AddUserAsync(context, 1, email, password, isUserVerified);
                await ManagerTestHelper.AddAuthTokenAsync(context, user.Id, deviceId, tokenString, true);

                await context.SaveChangesAsync();

                token = await sut.VerifyAccessToken(tokenString);
            }

            Assert.AreEqual(tokenString, token.Token);
            Assert.AreEqual(user.Id, token.UserId);
            Assert.AreEqual(isUserVerified, token.IsVerified);
        }

        [TestMethod]
        public async Task VerifyAccessTokenUserIsVerified()
        {
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            const string deviceId = "mydevice";
            const string tokenString = "randomtokenstring12345";
            AuthToken token;
            User user;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetAuthManager(context);

                user = await ManagerTestHelper.AddUserAsync(context, 1, email, password);
                await ManagerTestHelper.AddAuthTokenAsync(context, user.Id, deviceId, tokenString, true);

                await context.SaveChangesAsync();

                token = await sut.VerifyAccessToken(tokenString);
            }

            Assert.AreEqual(tokenString, token.Token);
            Assert.AreEqual(user.Id, token.UserId);
            Assert.IsTrue(token.IsVerified);
        }

        [TestMethod]
        public async Task VerifyAccessTokenTokenExpired()
        {
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            const string deviceId = "mydevice";
            const string tokenString = "randomtokenstring12345";
            AuthToken token;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetAuthManager(context);

                var user = await ManagerTestHelper.AddUserAsync(context, 1, email, password);
                await ManagerTestHelper.AddAuthTokenAsync(context, user.Id, deviceId, tokenString, false);

                await context.SaveChangesAsync();

                token = await sut.VerifyAccessToken(tokenString);
            }

            Assert.IsNull(token);
        }

        [TestMethod]
        public async Task VerifyAccessTokenUserNotFound()
        {
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            const string deviceId = "mydevice";
            const string tokenString = "randomtokenstring12345";
            AuthToken token;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetAuthManager(context);

                var user = await ManagerTestHelper.AddUserAsync(context, 1, email, password);
                await ManagerTestHelper.AddAuthTokenAsync(context, user.Id + 1, deviceId, tokenString, true);

                await context.SaveChangesAsync();

                token = await sut.VerifyAccessToken(tokenString);
            }

            Assert.IsNull(token);
        }

        [TestMethod]
        public async Task VerifyAccessTokenTokenNotFound()
        {
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            const string deviceId = "mydevice";
            const string tokenString = "randomtokenstring12345";
            AuthToken token;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetAuthManager(context);

                var user = await ManagerTestHelper.AddUserAsync(context, 1, email, password);
                await ManagerTestHelper.AddAuthTokenAsync(context, user.Id, deviceId, tokenString + "somesuffix", true);

                await context.SaveChangesAsync();

                token = await sut.VerifyAccessToken(tokenString);
            }

            Assert.IsNull(token);
        }

        private async Task<UserToken> GetAccessTokenInternal(bool isValid)
        {
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            const string deviceId = "mydevice";
            const string tokenString = "randomtokenstring12345";
            UserToken token;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetAuthManager(context);

                var user = await ManagerTestHelper.AddUserAsync(context, 1, email, password);
                await ManagerTestHelper.AddAuthTokenAsync(context, user.Id, deviceId, tokenString, isValid);

                await context.SaveChangesAsync();

                token = await sut.GetAccessToken(tokenString);
            }

            return token;
        }
    }
}
