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
    }
}
