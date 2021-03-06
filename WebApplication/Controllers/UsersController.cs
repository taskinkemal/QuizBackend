﻿using System.Net;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using Common;
using Microsoft.AspNetCore.Mvc;
using Models.TransferObjects;
using WebCommon.Attributes;
using WebCommon.BaseControllers;

namespace WebApplication.Controllers
{
    /// <summary>
    /// Controller for user related actions.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UsersController : NoAuthController
    {
        private readonly IUserManager userManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="userManager"></param>
        public UsersController(IUserManager userManager)
        {
            this.userManager = userManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("me")]
        public async Task<Models.DbModels.User> GetMe()
        {
            return await userManager.GetUserAsync(Token.UserId);
        }

        /// <summary>
        /// Add a user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <response code="406">Password Criteria Not Satisfied</response>
        /// <response code="409">Email Exists</response>
        [HttpPut]
        [ProducesResponseType(typeof(AuthToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.NotAcceptable)]
        public async Task<JsonResult> Put([FromBody] User user)
        {
            var result = await userManager.InsertUserAsync(user);

            switch (result.Response)
            {
                case InsertUserResponse.Success:
                    return ControllerHelper.CreateResponse(result.Value);
                case InsertUserResponse.EmailExists:
                    return ControllerHelper.CreateErrorResponse(HttpStatusCode.Conflict, "EmailExists");
                default: // case InsertUserResponse.PasswordCriteriaNotSatisfied:
                    return ControllerHelper.CreateErrorResponse(HttpStatusCode.NotAcceptable, "PasswordCriteriaNotSatisfied");
            }
        }

        /// <summary>
        /// Update user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <response code="401">Not authorized.</response>
        [HttpPost]
        [Authenticate]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.Unauthorized)]
        public async Task<JsonResult> Post([FromBody] User user)
        {
            var result = await userManager.UpdateUserAsync(Token.UserId, user);
            return result ? ControllerHelper.CreateResponse(true) : ControllerHelper.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");
        }

        /// <summary>
        /// Send verification email.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SendVerificationEmail")]
        public async Task<bool> PostSendVerificationEmail([FromBody] EmailRequest data)
        {
            var result = await userManager.SendAccountVerificationEmail(data.Email);
            return result;
        }

        /// <summary>
        /// Verify user account.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("VerifyAccount")]
        public async Task<AuthToken> PostVerifyAccount([FromBody] OneTimeTokenRequest data)
        {
            var token = await userManager.VerifyAccount(data);
            return token;
        }

        /// <summary>
        /// Delete user and all related references from the system. This action does not have a rollback.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<bool> Delete(int id)
        {
            //TODO: check rights.
            var result = await userManager.DeleteUserAsync(id);
            return result;
        }
    }
}
