using System;
using BusinessLayer.Context;
using BusinessLayer.Interfaces;
using Common;
using Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.DbModels;
using Models.TransferObjects;
using Serilog.Events;

namespace BusinessLayer.Implementations
{
    /// <summary>
    /// 
    /// </summary>
    public class UserManager : ManagerBase, IUserManager
    {
        private readonly IEmailManager emailManager;
        private readonly IAuthManager authManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="authManager"></param>
        /// <param name="logManager"></param>
        /// <param name="emailManager"></param>
        public UserManager(QuizContext context, IAuthManager authManager, ILogManager logManager, IEmailManager emailManager) : base(context, logManager)
        {
            this.authManager = authManager;
            this.emailManager = emailManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> SendPasswordResetEmail(string email)
        {
            var user = await Context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsVerified);

            if (user == null)
            {
                return false;
            }

            var token = AuthenticationHelper.GenerateRandomString(160);

            await Context.OneTimeTokens.AddAsync(new OneTimeToken
            {
                Email = email,
                Token = token,
                TokenType = (byte)OneTimeTokenType.ForgotPassword,
                ValidUntil = DateTime.Now.AddDays(1)
            });

            await Context.SaveChangesAsync();

            emailManager.Send(email, "Reset your password", "Here is your token: " + token);

            LogManager.AddLog(LogCategory.Email, "Password reset email sent: {email}", LogEventLevel.Information, email);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oneTimeToken"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> UpdatePassword(string oneTimeToken, string password)
        {
            if (!string.IsNullOrWhiteSpace(oneTimeToken))
            {
                var token = await Context.OneTimeTokens.FirstOrDefaultAsync(t =>
                    t.Token == oneTimeToken &&
                    t.TokenType == (byte)OneTimeTokenType.ForgotPassword &&
                    t.ValidUntil > DateTime.Now);

                if (token == null)
                {
                    return false;
                }

                var user = await Context.Users.FirstOrDefaultAsync(u => u.Email == token.Email && !u.IsVerified);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> UpdatePassword(int userId, string password)
        {
            if (!PasswordCriteria.IsValid(password))
            {
                return false;
            }

            var user = await Context.Users.FindAsync(userId);

            if (user == null)
            {
                return false;
            }

            user.PasswordHash = AuthenticationHelper.EncryptPassword(password);

            Context.Users.Update(user);

            await Context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> DeleteUserAsync(int userId)
        {
            var user = await Context.Users.FindAsync(userId);

            if (user == null)
            {
                return false;
            }

            Context.Users.Remove(user);

            await Context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<GenericManagerResponse<AuthToken, InsertUserResponse>> InsertUserAsync(Models.TransferObjects.User user)
        {
            var found = await Context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (found == null)
            {
                var userId = await InsertUserInternalAsync(user);

                if (userId == 0)
                {
                    return new GenericManagerResponse<AuthToken, InsertUserResponse>(InsertUserResponse.PasswordCriteriaNotSatisfied, null);
                }

                var token = await authManager.GenerateTokenAsync(userId, user.DeviceId);

                return new GenericManagerResponse<AuthToken, InsertUserResponse>(InsertUserResponse.Success, token);
            }

            return new GenericManagerResponse<AuthToken, InsertUserResponse>(InsertUserResponse.EmailExists, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> UpdateUserAsync(int userId, Models.TransferObjects.User user)
        {
            var found = await Context.Users.FindAsync(userId);

            if (found == null)
            {
                return false;
            }

            found.FirstName = user.FirstName;
            found.LastName = user.LastName;
            found.PictureUrl = user.PictureUrl;

            Context.Users.Update(found);

            await Context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Send account verification email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> SendAccountVerificationEmail(string email)
        {
            var user = await Context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsVerified);

            if (user == null)
            {
                return false;
            }

            var token = AuthenticationHelper.GenerateRandomString(160);

            await Context.OneTimeTokens.AddAsync(new OneTimeToken
            {
                Email = email,
                Token = token,
                TokenType = (byte)OneTimeTokenType.AccountVerification,
                ValidUntil = DateTime.Now.AddDays(1)
            });

            await Context.SaveChangesAsync();

            emailManager.Send(email, "Verify your account", "Here is your token: " + token);

            return true;
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
                t.TokenType == (byte)OneTimeTokenType.AccountVerification &&
                t.ValidUntil > DateTime.Now);

            if (oneTimeToken == null)
            {
                return null;
            }

            var user = await Context.Users.FirstOrDefaultAsync(u => u.Email == oneTimeToken.Email && !u.IsVerified);

            if (user == null)
            {
                return null;
            }

            var token = await authManager.GenerateTokenAsync(user.Id, request.DeviceId);

            Context.OneTimeTokens.Remove(oneTimeToken);

            user.IsVerified = true;

            Context.Update(user);

            await Context.SaveChangesAsync();

            return token;
        }

        private async System.Threading.Tasks.Task<int> InsertUserInternalAsync(Models.TransferObjects.User user)
        {
            if (!PasswordCriteria.IsValid(user.Password))
            {
                return 0;
            }

            var passwordHash = AuthenticationHelper.EncryptPassword(user.Password);

            var u = await Context.Users.AddAsync(new Models.DbModels.User
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
    }
}
