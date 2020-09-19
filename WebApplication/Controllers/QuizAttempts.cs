using System;
using System.Collections.Generic;
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
    public class QuizAttemptsController : AuthController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<QuizAttempt> Get()
        {
            var list = new List<QuizAttempt>();

            list.Add(new QuizAttempt
            {
                Id = 123,
                QuizId = 1,
                UserId = 1,
                StartDate = new DateTime(2020, 5, 23, 9, 5, 23),
                EndDate = new DateTime(2020, 5, 23, 9, 43, 33),
                Correct = 2,
                Incorrect = 1,
                Score = 78.5M,
                Result = QuizResult.Passed
            });

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<bool> Post(int id)
        {

            return true;
        }
    }
}
