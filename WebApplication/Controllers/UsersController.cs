using System.Threading.Tasks;
using BusinessLayer.Interfaces;
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
        private readonly IAuthManager authManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="authManager"></param>
        public UsersController(IAuthManager authManager)
        {
            this.authManager = authManager;
        }

        /// <summary>
        /// Add or update a user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AuthToken> Post([FromBody] User user)
        {
            var token = await authManager.UpsertUserAsync(user);
            return token;
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
            await authManager.SendAccountVerificationEmail(data.Email);
            return true;
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
            var token = await authManager.VerifyAccount(data);
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
            await authManager.DeleteUserAsync(id);
            return true;
        }
    }
}
