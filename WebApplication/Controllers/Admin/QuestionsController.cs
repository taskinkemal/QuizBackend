using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using Common;
using Microsoft.AspNetCore.Mvc;
using Models.DbModels;
using Models.TransferObjects;
using WebCommon.BaseControllers;

namespace WebApplication.Controllers.Admin
{
    /// <summary>
    /// 
    /// </summary>
    public class QuestionsController : ManagementController
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
        /// <returns></returns>
        [HttpGet]
        [Route("/Admin/Quizzes/{id}/Questions")]
        public async Task<List<Question>> Get(int id)
        {
            return await questionManager.GetQuizQuestions(Token.UserId, id);
        }
    }
}
