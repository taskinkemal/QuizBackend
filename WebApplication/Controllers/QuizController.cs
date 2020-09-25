using System.Collections.Generic;
using Models.DbModels;
using Microsoft.AspNetCore.Mvc;
using WebCommon.BaseControllers;
using BusinessLayer.Interfaces;
using System.Threading.Tasks;

namespace WebApplication.Controllers
{
    /// <summary>
    /// Quiz related actions.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class QuizController : AuthController
    {
        private readonly IQuizManager quizManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quizManager"></param>
        public QuizController(IQuizManager quizManager)
        {
            this.quizManager = quizManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Quiz>> Get()
        {
            return await quizManager.GetUserQuizList(Token.UserId);
        }
    }
}
