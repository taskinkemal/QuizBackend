using System;
using System.Text;
using BusinessLayer.Context;
using BusinessLayer.Interfaces;
using Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;


namespace BusinessLayer.Implementations
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthManager : ManagerBase, IAuthManager
    {
        private readonly ILogManager logManager;
        private const string Alphabet = "abcdefghijklmnoqprsqtuwxyz0123456789.";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logManager"></param>
        public AuthManager(QuizContext context, ILogManager logManager) : base(context)
        {
            this.logManager = logManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<UserToken> VerifyAccessToken(string accessToken)
        {
            var result = await Context.UserTokens.FirstOrDefaultAsync(t =>
                t.Token.Equals(accessToken, StringComparison.OrdinalIgnoreCase) &&
                t.ValidUntil > DateTime.Now);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<UserToken> GenerateTokenAsync(int userId, string deviceId)
        {
            UserToken result;

            var existingToken = await Context.UserTokens.FindAsync(userId, deviceId);

            if (existingToken != null)
            {
                existingToken.ValidUntil = DateTime.Now.AddYears(1);
                existingToken.Token = GenerateToken(userId);

                await Context.SaveChangesAsync();

                result = existingToken;
            }
            else
            {
                var newToken = new UserToken
                {
                    UserId = userId,
                    DeviceId = deviceId,
                    Token = GenerateToken(userId),
                    ValidUntil = DateTime.Now.AddYears(1)
                };

                Context.UserTokens.Add(newToken);

                await Context.SaveChangesAsync();

                result = newToken;
            }

            return result;
        }

        private async System.Threading.Tasks.Task<int> InsertUserAsync(User user)
        {
            var u = Context.Users.Add(new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PictureUrl = user.PictureUrl
            });

            await Context.SaveChangesAsync();

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
