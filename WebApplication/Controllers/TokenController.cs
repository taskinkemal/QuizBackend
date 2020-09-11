using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.TransferObjects;
using WebCommon.BaseControllers;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : NoAuthController
    {
        private readonly IAuthManager authManager;

        public TokenController(IAuthManager authManager)
        {
            this.authManager = authManager;
        }

        [HttpPost]
        public async Task<AuthToken> Post([FromBody] TokenRequest data)
        {
            var token = await authManager.GenerateTokenAsync(data);

            return token;
        }
    }
}
