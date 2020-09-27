using System;
using System.Collections.Generic;
using Models.DbModels;
using Microsoft.AspNetCore.Mvc;
using WebCommon.BaseControllers;
using BusinessLayer.Interfaces;
using System.Threading.Tasks;
using Models.TransferObjects;
using System.Net;
using Common;

namespace WebApplication.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class QuizAttemptsController : AuthController
    {
        private readonly IQuizAttemptManager quizAttemptManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quizAttemptManager"></param>
        public QuizAttemptsController(IQuizAttemptManager quizAttemptManager)
        {
            this.quizAttemptManager = quizAttemptManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <response code="401">User is not authorized to update the quiz attempt.</response>
        /// <response code="409">Quiz attempt status is not acceptable.</response>
        /// <response code="406">Quiz end date has passed.</response>
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.NotAcceptable)]
        [HttpPost]
        public async Task<JsonResult> Post([FromBody] QuizAttempt data)
        {
            var result = await quizAttemptManager.UpdateAttempt(Token.UserId, data);

            return
                result == UpdateQuizAttemptResponse.Success ? ControllerHelper.CreateResponse(true) :
                result == UpdateQuizAttemptResponse.NotAuthorized ? ControllerHelper.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized") :
                result == UpdateQuizAttemptResponse.StatusError ? ControllerHelper.CreateErrorResponse(HttpStatusCode.Conflict, "StatusError") :
                    ControllerHelper.CreateErrorResponse(HttpStatusCode.NotAcceptable, "DateError");
        }
    }
}
