using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.DbModels;
using WebCommon.BaseControllers;

namespace WebApplication.Controllers.Admin
{
    /// <summary>
    /// 
    /// </summary>
    public class QuizzesController : ManagementController
    {
        private readonly IQuizManager quizManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quizManager"></param>
        public QuizzesController(IQuizManager quizManager)
        {
            this.quizManager = quizManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<Quiz>> Get()
        {
            return await quizManager.GetAdminQuizList(Token.UserId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quiz"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<int> PutUpdateQuiz(int id, [FromBody] Quiz quiz)
        {
            return await quizManager.UpdateQuiz(Token.UserId, quiz);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quiz"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<int> PutInsertQuiz([FromBody] Quiz quiz)
        {
            return await quizManager.InsertQuiz(Token.UserId, quiz);
        }
    }
}
