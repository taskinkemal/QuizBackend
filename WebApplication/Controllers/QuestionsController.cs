using System.Collections.Generic;
using Models.DbModels;
using Microsoft.AspNetCore.Mvc;
using WebCommon.BaseControllers;
using BusinessLayer.Interfaces;
using System.Threading.Tasks;

namespace WebApplication.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class QuestionsController : AuthController
    {
        private readonly IQuestionManager questionManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="questionManager"></param>
        public QuestionsController(IQuestionManager questionManager)
        {
            this.questionManager = questionManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Quiz/{id}")]
        public async Task<IEnumerable<Question>> Get(int id)
        {
            return await questionManager.GetQuizQuestions(id);
        }
    }
}
