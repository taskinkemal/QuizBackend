using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.TransferObjects;
using WebCommon.Attributes;
using WebCommon.BaseControllers;

namespace WebApplication.Controllers
{
    /// <summary>
    /// Password related actions.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class PasswordController : NoAuthController
    {
        private readonly IUserManager userManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="userManager"></param>
        public PasswordController(IUserManager userManager)
        {
            this.userManager = userManager;
        }

        /// <summary>
        /// Password security criteria.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public PasswordCriteria Get()
        {
            return new PasswordCriteria();
        }

        /// <summary>
        /// Validate the given password against security criteria.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ValidatePassword")]
        public bool PostValidatePassword([FromBody] PasswordChangeRequest data)
        {
            return PasswordCriteria.IsValid(data.Password);
        }

        /// <summary>
        /// Send password reset request email.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SendPasswordResetEmail")]
        public async Task<bool> PostPasswordResetEmail([FromBody] EmailRequest data)
        {
            var result = await userManager.SendPasswordResetEmail(data.Email);
            return result;
        }

        /// <summary>
        /// Update user password. Either the access token or the one time token must be present. Access token has priority over the one time token.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authenticate]
        [HttpPost]
        public async Task<bool> Post([FromBody] PasswordChangeRequest data)
        {
            var result = await userManager.UpdatePassword(Token.UserId, data.Token, data.Password);
            return result;
        }
    }
}
