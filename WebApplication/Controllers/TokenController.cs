using System;
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
        /// <response code="401">Credentials are not verified.</response>
        [HttpPost]
        [ProducesResponseType(typeof(AuthToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.Unauthorized)]
        public async Task<JsonResult> Post([FromBody] TokenRequest data)
        {
            var token = await authManager.GenerateTokenAsync(data);

            return token != null ? ControllerHelper.CreateResponse(token) : ControllerHelper.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<JsonResult> Get(string token)
        {
            var result = await authManager.VerifyAccessToken(token);
            return ControllerHelper.CreateResponse(result != null && result.ValidUntil > DateTime.Now);
        }
    }
}
