using Models.DbModels;
using Microsoft.AspNetCore.Mvc;
using WebCommon.BaseControllers;

namespace WebApplication.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class AnswersController : AuthController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<bool> Post([FromBody] Answer answer)
        {

            return true;
        }
    }
}
