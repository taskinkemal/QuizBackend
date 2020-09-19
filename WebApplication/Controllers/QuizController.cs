using System.Collections.Generic;
using Models.DbModels;
using Microsoft.AspNetCore.Mvc;
using WebCommon.BaseControllers;

namespace WebApplication.Controllers
{
    /// <summary>
    /// Quiz related actions.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class QuizController : AuthController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Quiz> Get()
        {
            var list = new List<Quiz>();

            list.Add(new Quiz
            {
                Id = 1,
                Title = "Sample Quiz",
                Intro = "This is a sample quiz",
                Version = 1,
                TimeConstraint = true,
                TimeLimitInSeconds = 300,
                PoolIds = new List<int> { 3, 5 },
                ShuffleQuestions = true,
                ShuffleOptions = true,
                PassScore = 60,
                Repeatable = true,
                QuestionIds = new List<int> { 3, 4, 5 }
            });

            list.Add(new Quiz
            {
                Id = 2,
                Title = "Driving License Test",
                Intro = "This is a sample quiz for the driving license.",
                Version = 1,
                TimeConstraint = true,
                TimeLimitInSeconds = 600,
                PoolIds = new List<int> { 3 },
                ShuffleQuestions = true,
                ShuffleOptions = true,
                PassScore = 90,
                Repeatable = true,
                QuestionIds = new List<int> { 6, 7, 8, 9 }
            });

            return list;
        }
    }
}
