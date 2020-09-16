using System;
using System.Security.Cryptography;
using System.Text;
using BusinessLayer.Context;
using BusinessLayer.Interfaces;
using Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.DbModels;
using Models.TransferObjects;

namespace BusinessLayer.Implementations
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthManager : ManagerBase, IAuthManager
    {
        private readonly ILogManager logManager;
        private readonly IEmailManager emailManager;
        private const string Alphabet = "abcdefghijklmnoqprsqtuwxyz0123456789.";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logManager"></param>
        /// <param name="emailManager"></param>
        public AuthManager(QuizContext context, ILogManager logManager, IEmailManager emailManager) : base(context)
        {
            this.logManager = logManager;
            this.emailManager = emailManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<AuthToken> VerifyAccessToken(string accessToken)
        {
            var token = await GetAccessToken(accessToken);

            var user = await Context.Users.FindAsync(token.UserId);

            return new AuthToken
            {
                IsVerified = user.IsVerified,
                Token = token.Token,
                ValidUntil = token.ValidUntil
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> SendPasswordResetEmail(string email)
        {
            var user = await Context.Users.FirstAsync(u => u.Email == email && !u.IsVerified);

            var token = GenerateRandomString(160);

            Context.OneTimeTokens.Add(new Models.DbModels.OneTimeToken
            {
                Email = email,
                Token = token,
                TokenType = (byte)OneTimeTokenType.ForgotPassword,
                ValidUntil = DateTime.Now.AddDays(1)
            });

            await Context.SaveChangesAsync();

            if (user != null)
            {
                emailManager.Send(email, "Reset your password", "Here is your token: " + token);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="oneTimeToken"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> UpdatePassword(string accessToken, string oneTimeToken, string password)
        {
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                var token = await GetAccessToken(accessToken);

                if (token != null)
                {
                    var result = await UpdatePassword(token.UserId, password);
                    return result;
                }
            }
            else if (!string.IsNullOrWhiteSpace(oneTimeToken))
            {
                var token = await Context.OneTimeTokens.FirstOrDefaultAsync(t =>
                    t.Token == oneTimeToken &&
                    t.TokenType == (byte)OneTimeTokenType.ForgotPassword &&
                    t.ValidUntil > DateTime.Now);

                var user = await Context.Users.FirstAsync(u => u.Email == token.Email && !u.IsVerified);

                if (user != null)
                {
                    var result = await UpdatePassword(user.Id, password);

                    Context.OneTimeTokens.Remove(token);

                    await Context.SaveChangesAsync();

                    return result;
                }
            }

            return false;
        }

        private async System.Threading.Tasks.Task<bool> UpdatePassword(int userId, string password)
        {
            var user = await Context.Users.FindAsync(userId);

            user.PasswordHash = EncryptPassword(password);

            Context.Users.Update(user);

            await Context.SaveChangesAsync();

            return true;
        }

        private async System.Threading.Tasks.Task<AuthToken> GenerateTokenAsync(int userId, string deviceId)
        {
            AuthToken result;

            var existingToken = await Context.UserTokens.FindAsync(userId, deviceId);
            var user = await Context.Users.FindAsync(userId);

            if (existingToken != null)
            {
                existingToken.ValidUntil = DateTime.Now.AddYears(1);
                existingToken.Token = GenerateToken(userId);

                await Context.SaveChangesAsync();

                result = new AuthToken
                {
                    IsVerified = user.IsVerified,
                    Token = existingToken.Token,
                    ValidUntil = existingToken.ValidUntil
                };
            }
            else
            {
                var token = GenerateToken(userId);
                var newToken = new AuthToken
                {
                    IsVerified = user.IsVerified,
                    Token = token,
                    ValidUntil = DateTime.Now.AddYears(1)
                };

                Context.UserTokens.Add(new UserToken
                {
                    UserId = userId,
                    DeviceId = deviceId,
                    Token = token,
                    ValidUntil = DateTime.Now.AddYears(1)
                });

                await Context.SaveChangesAsync();

                result = newToken;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<AuthToken> GenerateTokenAsync(TokenRequest request)
        {
            byte[] passwordHash;

            using (var shaM = new SHA512Managed())
            {
                passwordHash = shaM.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
            }

            var user = await Context.Users.FirstAsync(u => u.Email == request.Email && u.PasswordHash == passwordHash);

            if (user != null)
            {
                var token = await GenerateTokenAsync(user.Id, request.DeviceId);

                return token;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> DeleteUserAsync(int userId)
        {
            var user = await Context.Users.FindAsync(userId);
            Context.Users.Remove(user);

            await Context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<AuthToken> UpsertUserAsync(Models.TransferObjects.User user)
        {
            var found = await Context.Users.FirstAsync(u => u.Email == user.Email);

            if (found == null)
            {
                var userId = await InsertUserInternalAsync(user);

                var token = await GenerateTokenAsync(userId, user.DeviceId);

                return token;
            }
            else
            {
                found.FirstName = user.FirstName;
                found.LastName = user.LastName;
                found.PictureUrl = user.PictureUrl;

                Context.Users.Update(found);

                await Context.SaveChangesAsync();

                return new AuthToken
                {
                    Token = ""
                };
            }
        }

        /// <summary>
        /// Send account verification email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> SendAccountVerificationEmail(string email)
        {
            var user = await Context.Users.FirstAsync(u => u.Email == email && !u.IsVerified);

            var token = GenerateRandomString(160);

            Context.OneTimeTokens.Add(new Models.DbModels.OneTimeToken
            {
                Email = email,
                Token = token,
                TokenType = (byte)OneTimeTokenType.AccountVerification,
                ValidUntil = DateTime.Now.AddDays(1)
            });

            await Context.SaveChangesAsync();

            if (user != null)
            {
                emailManager.Send(email, "Verify your account", "Here is your token: " + token);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<AuthToken> VerifyAccount(OneTimeTokenRequest request)
        {
            var oneTimeToken = await Context.OneTimeTokens.FirstOrDefaultAsync(t =>
                t.Token == request.Token &&
                t.ValidUntil > DateTime.Now);

            var user = await Context.Users.FirstAsync(u => u.Email == oneTimeToken.Email && !u.IsVerified);

            var token = await GenerateTokenAsync(user.Id, request.DeviceId);

            Context.OneTimeTokens.Remove(oneTimeToken);

            user.IsVerified = true;

            Context.Update(user);

            await Context.SaveChangesAsync();

            return token;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<Models.DbModels.UserToken> GetAccessToken(string accessToken)
        {
            var token = await Context.UserTokens.FirstOrDefaultAsync(t =>
                t.Token == accessToken &&
                t.ValidUntil > DateTime.Now);

            return token;
        }

        private byte[] EncryptPassword(string password)
        {
            byte[] passwordHash;

            using (var shaM = new SHA512Managed())
            {
                passwordHash = shaM.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            return passwordHash;
        }

        private async System.Threading.Tasks.Task<int> InsertUserInternalAsync(Models.TransferObjects.User user)
        {
            var passwordHash = EncryptPassword(user.Password);

            var u = Context.Users.Add(new Models.DbModels.User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PictureUrl = user.PictureUrl,
                PasswordHash = passwordHash,
                IsVerified = false
            });

            await Context.SaveChangesAsync();

            await SendAccountVerificationEmail(user.Email);

            return u.Entity.Id;
        }

        private static string GenerateToken(int userID)
        {
            return GenerateRandomString(160) + "_" + userID;
        }

        private static string GenerateRandomString(int length)
        {
            var rand = new Random(DateTime.Now.Millisecond);
            var sb = new StringBuilder();
            for (var i = 0; i < length; i++)
                sb.Append(Alphabet[rand.Next(Alphabet.Length)]);

            return sb.ToString();
        }
    }
}
