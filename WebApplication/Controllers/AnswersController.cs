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
        /// <param name="id"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("QuizAttempts/{id}")]
        public ActionResult<bool> Post(int id, [FromBody] Models.TransferObjects.Answer answer)
        {

            return true;
        }
    }
}
