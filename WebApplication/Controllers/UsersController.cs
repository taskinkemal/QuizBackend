using System.Net;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using Common;
using Microsoft.AspNetCore.Mvc;
using Models.TransferObjects;
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
        /// Add a user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(AuthToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.Unauthorized)]
        public async Task<JsonResult> Put([FromBody] User user)
        {
            var result = await userManager.InsertUserAsync(user);

            switch (result.Response)
            {
                case InsertUserResponse.Success:
                    return CreateResponse(result.Value);
                case InsertUserResponse.EmailExists:
                    return CreateErrorResponse(HttpStatusCode.Conflict, "EmailExists");
                case InsertUserResponse.PasswordCriteriaNotSatisfied:
                    return CreateErrorResponse(HttpStatusCode.NotAcceptable, "PasswordCriteriaNotSatisfied");
                default:
                    return CreateErrorResponse(HttpStatusCode.InternalServerError, "SystemError");
            }
        }

        /// <summary>
        /// Update user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.Unauthorized)]
        public async Task<JsonResult> Post([FromBody] User user)
        {
            var result = await userManager.UpdateUserAsync(AccessTokenString, user);
            return result ? CreateResponse(true) : CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");
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
            var result = await userManager.DeleteUserAsync(id);
            return result;
        }
    }
}
