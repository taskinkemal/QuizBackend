using System;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.TransferObjects;
using WebCommon.BaseControllers;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PasswordController : NoAuthController
    {
        private readonly IAuthManager authManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="authManager"></param>
        public PasswordController(IAuthManager authManager)
        {
            this.authManager = authManager;
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
        /// Send password reset request email.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SendPasswordResetEmail")]
        public async Task<bool> PostPasswordResetEmail([FromBody] EmailRequest data)
        {
            var result = await authManager.SendPasswordResetEmail(data.Email);
            return result;
        }

        /// <summary>
        /// Update user password
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> Post([FromBody] PasswordChangeRequest data)
        {
            var result = await authManager.UpdatePassword(this.Token?.IsVerified == true ? this.Token.Token : "", data.Token, data.Password);
            return result;
        }
    }
}
