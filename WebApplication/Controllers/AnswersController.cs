using Microsoft.AspNetCore.Mvc;
using WebCommon.BaseControllers;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using System.Net;
using Common;
using Models.TransferObjects;

namespace WebApplication.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class AnswersController : AuthController
    {
        private readonly IQuizAttemptManager quizAttemptManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quizAttemptManager"></param>
        public AnswersController(IQuizAttemptManager quizAttemptManager)
        {
            this.quizAttemptManager = quizAttemptManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        /// <response code="401">User is not authorized to add the answer.</response>
        /// <response code="409">Quiz attempt status is not acceptable.</response>
        /// <response code="406">Quiz end date has passed.</response>
        /// <response code="417">Time up.</response>
        [ProducesResponseType(typeof(UpdateQuizAttemptStatusResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.ExpectationFailed)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.NotAcceptable)]
        [HttpPost]
        [Route("Answers/{id}")]
        public async Task<JsonResult> Post(int id, [FromBody] Answer answer)
        {
            var result = await quizAttemptManager.InsertAnswerAsync(Token.UserId, id, answer);

            switch (result)
            {
                case UpdateQuizAttemptStatusResult.DateError:
                    return ControllerHelper.CreateErrorResponse(HttpStatusCode.NotAcceptable, "DateError");

                case UpdateQuizAttemptStatusResult.NotAuthorized:
                    return ControllerHelper.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");

                case UpdateQuizAttemptStatusResult.StatusError:
                    return ControllerHelper.CreateErrorResponse(HttpStatusCode.Conflict, "StatusError");

                case UpdateQuizAttemptStatusResult.TimeUp:
                    return ControllerHelper.CreateErrorResponse(HttpStatusCode.ExpectationFailed, "TimeUp");

                default:
                    return ControllerHelper.CreateResponse(result);
            }
        }
    }
}
