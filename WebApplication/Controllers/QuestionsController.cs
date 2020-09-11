using System.Collections.Generic;
using Models.DbModels;
using Microsoft.AspNetCore.Mvc;
using WebCommon.BaseControllers;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionsController : AuthController
    {
        [HttpGet]
        [Route("Quiz/{id}")]
        public IEnumerable<Question> Get(int id)
        {
            var list = new List<Question>();

            if (id == 1)
            {
                list.Add(new Question
                {
                    Id = 3,
                    Body = "This is a question. Correct answer is easy to find.",
                    Level = 3,
                    PoolIds = new List<int> { 3 },
                    QuestionType = QuestionType.MultiSelect,
                    OptionIds = new List<int> { 2, 3, 4, 5 },
                    CorrectOptionIds = new List<int> { 3 },
                    Tags = new List<string> { "Sample", "Easy Questions" }
                });

                list.Add(new Question
                {
                    Id = 4,
                    Body = "Is this true or false.",
                    Level = 2,
                    PoolIds = new List<int> { 3 },
                    QuestionType = QuestionType.YesNo,
                    CorrectOptionIds = new List<int> { 1 },
                    Tags = new List<string> { "Sample", "Easy Questions" }
                });

                list.Add(new Question
                {
                    Id = 5,
                    Body = "This is a multi answer question. Correct answers is easy to find.",
                    Level = 3,
                    PoolIds = new List<int> { 3 },
                    QuestionType = QuestionType.MultiSelect,
                    OptionIds = new List<int> { 8, 9, 10, 11 },
                    CorrectOptionIds = new List<int> { 8, 10 },
                    Tags = new List<string> { "Sample", "Easy Questions" }
                });
            }

            return list;
        }
    }
}
