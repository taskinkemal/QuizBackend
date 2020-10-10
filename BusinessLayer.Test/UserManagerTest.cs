using System.Threading.Tasks;
using BusinessLayer.Context;
using BusinessLayer.Interfaces;
using Common;
using Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;
using Models.TransferObjects;
using Moq;
using Serilog.Events;

namespace BusinessLayer.Test
{
    [TestClass]
    public class UserManagerTest
    {
        [TestMethod]
        public async Task GetUser()
        {
            var logManager = new Mock<ILogManager>();
            int userId;
            Models.DbModels.User user = null;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>(), logManager.Object);

                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(0), true);
                userId = await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(1), true);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(2), true);

                await context.SaveChangesAsync();

                user = await sut.GetUserAsync(userId);
            }

            Assert.AreEqual(userId, user.Id);
        }

        [TestMethod]
        public async Task GetUserNull()
        {
            var logManager = new Mock<ILogManager>();
            int userId;
            Models.DbModels.User user = null;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>(), logManager.Object);

                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(0), true);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(1), true);
                userId = await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(2), true);

                await context.SaveChangesAsync();

                user = await sut.GetUserAsync(userId + 1);
            }

            Assert.IsNull(user);
        }

        [TestMethod]
        public async Task SendPasswordResetEmailCreatesToken()
        {
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            bool result;
            OneTimeToken token;
            var isLogManagerCalled = false;

            var logManager = new Mock<ILogManager>();

            logManager.Setup(c => c.AddLog(LogCategory.Email, It.IsAny<string>(), LogEventLevel.Information, It.IsAny<object[]>()))
                .Callback(() =>
                {
                    isLogManagerCalled = true;
                });

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>(), logManager.Object);

                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(0), true);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(1, email, password), false);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(2), true);

                await context.SaveChangesAsync();

                result = await sut.SendPasswordResetEmail(email);

                token = await context.OneTimeTokens.FirstOrDefaultAsync(u => u.Email == email);
            }

            Assert.IsTrue(result);
            Assert.IsNotNull(token);
            Assert.IsTrue(isLogManagerCalled);
        }

        [TestMethod]
        public async Task SendPasswordResetEmailNoEmail()
        {
            bool result;
            OneTimeToken token;
            const string email = "nonexisting@mymail.com";

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(0), true);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(1), true);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(2), true);

                await context.SaveChangesAsync();

                result = await sut.SendPasswordResetEmail(email);

                token = await context.OneTimeTokens.FirstOrDefaultAsync(u => u.Email == email);
            }

            Assert.IsFalse(result);
            Assert.IsNull(token);
        }

        [TestMethod]
        public async Task SendPasswordResetEmailVerified()
        {
            bool result;
            OneTimeToken token;
            const string password = "mypassword123";
            const string email = "user1@mymail.com";

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(0), true);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(1, email, password), true);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(2), true);

                await context.SaveChangesAsync();

                result = await sut.SendPasswordResetEmail(email);

                token = await context.OneTimeTokens.FirstOrDefaultAsync(u => u.Email == email);
            }

            Assert.IsFalse(result);
            Assert.IsNull(token);
        }

        [TestMethod]
        public async Task UpdatePasswordExistingUser()
        {
            bool result;
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            const string newPassword = "newPassword123";
            Models.DbModels.User updatedUser;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await ManagerTestHelper.AddUserAsync(context, 0);
                var user = await ManagerTestHelper.AddUserAsync(context, 1, email, password, true);
                await ManagerTestHelper.AddUserAsync(context, 2);

                await context.SaveChangesAsync();

                result = await sut.UpdatePassword(user.Id, newPassword);

                updatedUser = await context.Users.FindAsync(user.Id);
            }

            Assert.IsTrue(result);
            Assert.IsTrue(AuthenticationHelper.CompareByteArrays(AuthenticationHelper.EncryptPassword(newPassword), updatedUser.PasswordHash));
        }

        [TestMethod]
        public async Task UpdatePasswordInvalidPassword()
        {
            bool result;
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            const string newPassword = "a";
            Models.DbModels.User updatedUser;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await ManagerTestHelper.AddUserAsync(context, 0);
                var user = await ManagerTestHelper.AddUserAsync(context, 1, email, password, true);
                await ManagerTestHelper.AddUserAsync(context, 2);

                await context.SaveChangesAsync();

                result = await sut.UpdatePassword(user.Id, newPassword);

                updatedUser = await context.Users.FindAsync(user.Id);
            }

            Assert.IsFalse(result);
            Assert.IsTrue(AuthenticationHelper.CompareByteArrays(AuthenticationHelper.EncryptPassword(password), updatedUser.PasswordHash));
        }

        [TestMethod]
        public async Task UpdatePasswordInvalidUser()
        {
            bool result;
            const string password = "mypassword123";
            const string email = "user1@mymail.com";

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await ManagerTestHelper.AddUserAsync(context, 0);
                var user = await ManagerTestHelper.AddUserAsync(context, 1, email, password, true);
                
                await context.SaveChangesAsync();

                result = await sut.UpdatePassword(user.Id+1, password);
            }

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task UpdatePasswordWithToken()
        {
            bool result;
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            const string newPassword = "newPassword123";
            const string tokenString = "token";
            Models.DbModels.User updatedUser;
            OneTimeToken deletedToken;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await ManagerTestHelper.AddUserAsync(context, 0);
                var user = await ManagerTestHelper.AddUserAsync(context, 1, email, password, false);
                await ManagerTestHelper.AddUserAsync(context, 2);
                await ManagerTestHelper.AddOneTimeTokenAsync(context, email, OneTimeTokenType.ForgotPassword, tokenString, true);

                await context.SaveChangesAsync();

                result = await sut.UpdatePassword(tokenString, newPassword);

                updatedUser = await context.Users.FindAsync(user.Id);
                deletedToken = await context.OneTimeTokens.FirstOrDefaultAsync(t => t.Email == email && t.Token == tokenString);
            }

            Assert.IsTrue(result);
            Assert.IsTrue(AuthenticationHelper.CompareByteArrays(AuthenticationHelper.EncryptPassword(newPassword), updatedUser.PasswordHash));
            Assert.IsNull(deletedToken);
        }

        [TestMethod]
        public async Task UpdatePasswordWithExpiredToken()
        {
            bool result;
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            const string newPassword = "newPassword123";
            const string tokenString = "token";
            Models.DbModels.User updatedUser;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await ManagerTestHelper.AddUserAsync(context, 0);
                var user = await ManagerTestHelper.AddUserAsync(context, 1, email, password, false);
                await ManagerTestHelper.AddUserAsync(context, 2);
                await ManagerTestHelper.AddOneTimeTokenAsync(context, email, OneTimeTokenType.ForgotPassword, tokenString, false);

                await context.SaveChangesAsync();

                result = await sut.UpdatePassword(tokenString, newPassword);

                updatedUser = await context.Users.FindAsync(user.Id);
            }

            Assert.IsFalse(result);
            Assert.IsTrue(AuthenticationHelper.CompareByteArrays(AuthenticationHelper.EncryptPassword(password), updatedUser.PasswordHash));
        }

        [TestMethod]
        public async Task UpdatePasswordWithTokenUserVerified()
        {
            bool result;
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            const string newPassword = "newPassword123";
            const string tokenString = "token";
            Models.DbModels.User updatedUser;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await ManagerTestHelper.AddUserAsync(context, 0);
                var user = await ManagerTestHelper.AddUserAsync(context, 1, email, password, true);
                await ManagerTestHelper.AddUserAsync(context, 2);
                await ManagerTestHelper.AddOneTimeTokenAsync(context, email, OneTimeTokenType.ForgotPassword, tokenString, true);

                await context.SaveChangesAsync();

                result = await sut.UpdatePassword(tokenString, newPassword);

                updatedUser = await context.Users.FindAsync(user.Id);
            }

            Assert.IsFalse(result);
            Assert.IsTrue(AuthenticationHelper.CompareByteArrays(AuthenticationHelper.EncryptPassword(password), updatedUser.PasswordHash));
        }

        [TestMethod]
        public async Task UpdatePasswordWithTokenUserNotFound()
        {
            bool result;
            const string email = "nonexisting@mymail.com";
            const string newPassword = "newPassword123";
            const string tokenString = "token";
            
            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(0), true);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(1), true);
                await ManagerTestHelper.AddOneTimeTokenAsync(context, email, OneTimeTokenType.ForgotPassword, tokenString, true);

                await context.SaveChangesAsync();

                result = await sut.UpdatePassword(tokenString, newPassword);
            }

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task UpdatePasswordWithTokenWrongType()
        {
            bool result;
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            const string newPassword = "newPassword123";
            const string tokenString = "token";
            Models.DbModels.User updatedUser;
            
            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await ManagerTestHelper.AddUserAsync(context, 0);
                var user = await ManagerTestHelper.AddUserAsync(context, 1, email, password, false);
                await ManagerTestHelper.AddUserAsync(context, 2);
                await ManagerTestHelper.AddOneTimeTokenAsync(context, email, OneTimeTokenType.AccountVerification, tokenString, true);

                await context.SaveChangesAsync();

                result = await sut.UpdatePassword(tokenString, newPassword);

                updatedUser = await context.Users.FindAsync(user.Id);
            }

            Assert.IsFalse(result);
            Assert.IsTrue(AuthenticationHelper.CompareByteArrays(AuthenticationHelper.EncryptPassword(password), updatedUser.PasswordHash));
        }

        [TestMethod]
        public async Task DeleteUser()
        {
            bool result;
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            Models.DbModels.User deletedUser;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await ManagerTestHelper.AddUserAsync(context, 0);
                var user = await ManagerTestHelper.AddUserAsync(context, 1, email, password, false);
                await ManagerTestHelper.AddUserAsync(context, 2);
                
                await context.SaveChangesAsync();

                result = await sut.DeleteUserAsync(user.Id);

                deletedUser = await context.Users.FindAsync(user.Id);
            }

            Assert.IsTrue(result);
            Assert.IsNull(deletedUser);
        }

        [TestMethod]
        public async Task DeleteUserNotFound()
        {
            bool result;
            const string password = "mypassword123";
            const string email = "user1@mymail.com";

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await ManagerTestHelper.AddUserAsync(context, 0);
                var user = await ManagerTestHelper.AddUserAsync(context, 1, email, password, false);

                await context.SaveChangesAsync();

                result = await sut.DeleteUserAsync(user.Id + 1);
            }

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task InsertUser()
        {
            GenericManagerResponse<AuthToken, InsertUserResponse> result;
            const string password = "mypassword123";
            const string email = "user1@mymail.com";

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(0), true);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(1, email, password), false);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(2), true);

                await context.SaveChangesAsync();

                result = await sut.InsertUserAsync(new Models.TransferObjects.User
                {
                    DeviceId = "device",
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Email = "newemail@myemail.com",
                    PictureUrl = "",
                    Password = "password111"
                });
            }

            Assert.AreEqual(result.Response, InsertUserResponse.Success);
        }

        [TestMethod]
        public async Task InsertUserExistingEmail()
        {
            GenericManagerResponse<AuthToken, InsertUserResponse> result;
            const string password = "mypassword123";
            const string email = "user1@mymail.com";

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(0), true);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(1, email, password), false);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(2), true);

                await context.SaveChangesAsync();

                result = await sut.InsertUserAsync(new Models.TransferObjects.User
                {
                    DeviceId = "device",
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Email = email,
                    PictureUrl = "",
                    Password = "password111"
                });
            }

            Assert.AreEqual(result.Response, InsertUserResponse.EmailExists);
        }

        [TestMethod]
        public async Task InsertUserInvalidPassword()
        {
            GenericManagerResponse<AuthToken, InsertUserResponse> result;
            const string password = "mypassword123";
            const string email = "user1@mymail.com";

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(0), true);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(1, email, password), false);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(2), true);

                await context.SaveChangesAsync();

                result = await sut.InsertUserAsync(new Models.TransferObjects.User
                {
                    DeviceId = "device",
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Email = "newemail@myemail.com",
                    PictureUrl = "",
                    Password = "p"
                });
            }

            Assert.AreEqual(result.Response, InsertUserResponse.PasswordCriteriaNotSatisfied);
        }

        [TestMethod]
        public async Task UpdateUser()
        {
            bool result;
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            const string newFirstName = "newName";
            const string newLastName = "newLastName";
            const string newPictureUrl = "newUrl";
            Models.DbModels.User updatedUser;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await ManagerTestHelper.AddUserAsync(context, 0);
                var user = await ManagerTestHelper.AddUserAsync(context, 1, email, password, false);
                await ManagerTestHelper.AddUserAsync(context, 2);

                await context.SaveChangesAsync();

                result = await sut.UpdateUserAsync(user.Id, new Models.TransferObjects.User
                {
                    DeviceId = "device",
                    FirstName = newFirstName,
                    LastName = newLastName,
                    Email = email,
                    PictureUrl = newPictureUrl,
                    Password = "p"
                });

                updatedUser = await context.Users.FindAsync(user.Id);
            }

            Assert.IsTrue(result);
            Assert.AreEqual(updatedUser.FirstName, newFirstName);
            Assert.AreEqual(updatedUser.LastName, newLastName);
            Assert.AreEqual(updatedUser.PictureUrl, newPictureUrl);
        }

        [TestMethod]
        public async Task UpdateUserInvalidUser()
        {
            bool result;
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            const string newFirstName = "newName";
            const string newLastName = "newLastName";
            const string newPictureUrl = "newUrl";

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await ManagerTestHelper.AddUserAsync(context, 0);
                var user = await ManagerTestHelper.AddUserAsync(context, 1, email, password, false);
                
                await context.SaveChangesAsync();

                result = await sut.UpdateUserAsync(user.Id + 1, new Models.TransferObjects.User
                {
                    DeviceId = "device",
                    FirstName = newFirstName,
                    LastName = newLastName,
                    Email = email,
                    PictureUrl = newPictureUrl,
                    Password = "p"
                });
            }

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task SendAccountVerificationEmail()
        {
            bool result;
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            OneTimeToken token;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(0), true);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(1, email, password), false);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(2), true);

                await context.SaveChangesAsync();

                result = await sut.SendAccountVerificationEmail(email);

                token = await context.OneTimeTokens.FirstOrDefaultAsync(t => t.Email == email && t.TokenType == (byte)OneTimeTokenType.AccountVerification);
            }

            Assert.IsTrue(result);
            Assert.IsNotNull(token);
        }

        [TestMethod]
        public async Task SendAccountVerificationEmailUserVerified()
        {
            bool result;
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            OneTimeToken token;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(0), true);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(1, email, password), true);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(2), true);

                await context.SaveChangesAsync();

                result = await sut.SendAccountVerificationEmail(email);

                token = await context.OneTimeTokens.FirstOrDefaultAsync(t => t.Email == email && t.TokenType == (byte)OneTimeTokenType.AccountVerification);
            }

            Assert.IsFalse(result);
            Assert.IsNull(token);
        }

        [TestMethod]
        public async Task SendAccountVerificationEmailUserNotFound()
        {
            bool result;
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            const string nonExistingEmail = "nonexisting@mymail.com";
            OneTimeToken token;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(0), true);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(1, email, password), false);

                await context.SaveChangesAsync();

                result = await sut.SendAccountVerificationEmail(nonExistingEmail);

                token = await context.OneTimeTokens.FirstOrDefaultAsync(t => t.Email == nonExistingEmail && t.TokenType == (byte)OneTimeTokenType.AccountVerification);
            }

            Assert.IsFalse(result);
            Assert.IsNull(token);
        }

        [TestMethod]
        public async Task VerifyAccount()
        {
            AuthToken result;
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            const string tokenString = "token";
            OneTimeToken token;
            Models.DbModels.User user;
            Models.DbModels.User updatedUser;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var authManager = new Mock<IAuthManager>();
                authManager
                    .Setup(c => c.GenerateTokenAsync(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns((int u, string d) => Task.FromResult(new AuthToken
                    {
                        UserId = u
                    }));
                var sut = ManagerTestHelper.GetUserManager(context, authManager.Object);

                await ManagerTestHelper.AddUserAsync(context, 0);
                user = await ManagerTestHelper.AddUserAsync(context, 1, email, password, false);
                await ManagerTestHelper.AddUserAsync(context, 2);
                await ManagerTestHelper.AddOneTimeTokenAsync(context, email, OneTimeTokenType.AccountVerification, tokenString, true);

                await context.SaveChangesAsync();

                result = await sut.VerifyAccount(new OneTimeTokenRequest
                {
                    Token = tokenString,
                    DeviceId = "device"
                });

                token = await context.OneTimeTokens.FirstOrDefaultAsync(t => t.Email == email && t.TokenType == (byte)OneTimeTokenType.AccountVerification);
                updatedUser = await context.Users.FindAsync(user.Id);
            }

            Assert.AreEqual(result.UserId, user.Id);
            Assert.IsNull(token);
            Assert.IsTrue(updatedUser.IsVerified);
        }

        [TestMethod]
        public async Task VerifyAccountInvalidToken()
        {
            AuthToken result;
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            const string tokenString = "token";

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(0), true);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(1, email, password), false);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(2), true);
                await ManagerTestHelper.AddOneTimeTokenAsync(context, email, OneTimeTokenType.AccountVerification, "differentToken", true);

                await context.SaveChangesAsync();

                result = await sut.VerifyAccount(new OneTimeTokenRequest
                {
                    Token = tokenString,
                    DeviceId = "device"
                });
            }

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task VerifyAccountExpiredToken()
        {
            AuthToken result;
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            const string tokenString = "token";

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(0), true);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(1, email, password), false);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(2), true);
                await ManagerTestHelper.AddOneTimeTokenAsync(context, email, OneTimeTokenType.AccountVerification, tokenString, false);

                await context.SaveChangesAsync();

                result = await sut.VerifyAccount(new OneTimeTokenRequest
                {
                    Token = tokenString,
                    DeviceId = "device"
                });
            }

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task VerifyAccountNoToken()
        {
            AuthToken result;
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            const string tokenString = "token";

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(0), true);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(1, email, password), false);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(2), true);
                await ManagerTestHelper.AddOneTimeTokenAsync(context, "differentEmail@mymail.com", OneTimeTokenType.AccountVerification, tokenString, false);

                await context.SaveChangesAsync();

                result = await sut.VerifyAccount(new OneTimeTokenRequest
                {
                    Token = tokenString,
                    DeviceId = "device"
                });
            }

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task VerifyAccountAlreadyVerified()
        {
            AuthToken result;
            const string password = "mypassword123";
            const string email = "user1@mymail.com";
            const string tokenString = "token";

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var sut = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>());

                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(0), true);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(1, email, password), true);
                await sut.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(2), true);
                await ManagerTestHelper.AddOneTimeTokenAsync(context, email, OneTimeTokenType.AccountVerification, tokenString, true);

                await context.SaveChangesAsync();

                result = await sut.VerifyAccount(new OneTimeTokenRequest
                {
                    Token = tokenString,
                    DeviceId = "device"
                });
            }

            Assert.IsNull(result);
        }
    }
}