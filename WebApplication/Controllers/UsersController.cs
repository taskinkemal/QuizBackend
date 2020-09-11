using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.TransferObjects;
using WebCommon.BaseControllers;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : NoAuthController
    {
        private readonly IAuthManager authManager;

        public UsersController(IAuthManager authManager)
        {
            this.authManager = authManager;
        }

        [HttpPost]
        public async Task<AuthToken> Post([FromBody] User user)
        {
            var token = await authManager.UpsertUserAsync(user);
            return token;
        }

        [HttpPost]
        [Route("SendVerificationEmail")]
        public async Task<bool> PostSendVerificationEmail([FromBody] AccountVerificationRequest data)
        {
            await authManager.SendAccountVerificationEmail(data.Email);
            return true;
        }

        [HttpPost]
        [Route("VerifyAccount")]
        public async Task<AuthToken> PostSendVerificationEmail([FromBody] OneTimeTokenRequest data)
        {
            var token = await authManager.VerifyAccount(data);
            return token;
        }

        [HttpDelete]
        public async Task<bool> Delete(int id)
        {
            await authManager.DeleteUserAsync(id);
            return true;
        }
    }
}
