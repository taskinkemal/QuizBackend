﻿using System;
using BusinessLayer.Context;
using BusinessLayer.Interfaces;
using Common;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logManager"></param>
        public AuthManager(QuizContext context, ILogManager logManager) : base(context, logManager)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<AuthToken> VerifyAccessToken(string accessToken)
        {
            var token = await GetAccessToken(accessToken);

            if (token == null)
            {
                return null;
            }

            var user = await Context.Users.FindAsync(token.UserId);

            if (user == null)
            {
                return null;
            }

            return new AuthToken
            {
                IsVerified = user.IsVerified,
                Token = token.Token,
                ValidUntil = token.ValidUntil,
                UserId = user.Id
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<AuthToken> GenerateTokenAsync(TokenRequest request)
        {
            var passwordHash = AuthenticationHelper.EncryptPassword(request.Password);

            var user = await Context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user != null && AuthenticationHelper.CompareByteArrays(user.PasswordHash, passwordHash))
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
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<AuthToken> GenerateTokenAsync(int userId, string deviceId)
        {
            AuthToken result;

            var existingToken = await Context.UserTokens.FindAsync(userId, deviceId);
            var user = await Context.Users.FindAsync(userId);

            if (user == null)
            {
                return null;
            }

            if (existingToken != null)
            {
                existingToken.ValidUntil = DateTime.Now.AddYears(1);
                existingToken.Token = AuthenticationHelper.GenerateToken(userId);

                await Context.SaveChangesAsync();

                result = new AuthToken
                {
                    UserId = user.Id,
                    IsVerified = user.IsVerified,
                    Token = existingToken.Token,
                    ValidUntil = existingToken.ValidUntil
                };
            }
            else
            {
                var token = AuthenticationHelper.GenerateToken(userId);

                await Context.UserTokens.AddAsync(new UserToken
                {
                    UserId = userId,
                    DeviceId = deviceId,
                    Token = token,
                    ValidUntil = DateTime.Now.AddYears(1)
                });

                await Context.SaveChangesAsync();

                result = new AuthToken
                {
                    UserId = user.Id,
                    IsVerified = user.IsVerified,
                    Token = token,
                    ValidUntil = DateTime.Now.AddYears(1)
                };
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<UserToken> GetAccessToken(string accessToken)
        {
            var token = await Context.UserTokens.FirstOrDefaultAsync(t =>
                t.Token == accessToken &&
                t.ValidUntil > DateTime.Now);

            return token;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> DeleteAccessToken(string accessToken)
        {
            var token = await Context.UserTokens.FirstOrDefaultAsync(t =>
                t.Token == accessToken);

            if (token == null)
            {
                return false;
            }

            Context.UserTokens.Remove(token);

            await Context.SaveChangesAsync();

            return true;
        }
    }
}
