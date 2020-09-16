using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.TransferObjects;
using WebCommon.BaseControllers;

namespace WebApplication.Controllers
{
    /// <summary>
    /// Authentication token functionality.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class TokenController : NoAuthController
    {
        private readonly IAuthManager authManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="authManager"></param>
        public TokenController(IAuthManager authManager)
        {
            this.authManager = authManager;
        }

        /// <summary>
        /// Verify the credentials and generate a token.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AuthToken> Post([FromBody] TokenRequest data)
        {
            var token = await authManager.GenerateTokenAsync(data);

            return token;
        }
    }
}
