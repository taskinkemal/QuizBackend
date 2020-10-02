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
        /// Starts / restarts a quiz
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="401">User is not authorized to add the quiz attempt.</response>
        /// <response code="409">Quiz is not repeatable.</response>
        /// <response code="406">Quiz end date has passed.</response>
        [ProducesResponseType(typeof(QuizAttempt), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.NotAcceptable)]
        [HttpPut]
        [Route("QuizAttempts/{id}")]
        public async Task<JsonResult> Put(int id)
        {
            var result = await quizAttemptManager.CreateAttempt(Token.UserId, id);

            return
                result.Result == CreateAttemptResult.Success ? ControllerHelper.CreateResponse(result.Attempt) :
                result.Result == CreateAttemptResult.NotAuthorized ? ControllerHelper.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized") :
                result.Result == CreateAttemptResult.NotRepeatable ? ControllerHelper.CreateErrorResponse(HttpStatusCode.Conflict, "NotRepeatable") :
                    ControllerHelper.CreateErrorResponse(HttpStatusCode.NotAcceptable, "DateError");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <response code="401">User is not authorized to update the quiz attempt.</response>
        /// <response code="409">Quiz attempt status is not acceptable.</response>
        /// <response code="406">Quiz end date has passed.</response>
        [ProducesResponseType(typeof(QuizAttempt), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.NotAcceptable)]
        [HttpPost]
        [Route("QuizAttempts/Status/{id}")]
        public async Task<JsonResult> Post(int id, [FromBody] UpdateQuizAttemptStatus data)
        {
            var result = await quizAttemptManager.UpdateStatus(Token.UserId, id, data);

            return
                result.Result == UpdateQuizAttemptStatusResult.Success ? ControllerHelper.CreateResponse(result.Attempt) :
                result.Result == UpdateQuizAttemptStatusResult.NotAuthorized ? ControllerHelper.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized") :
                result.Result == UpdateQuizAttemptStatusResult.StatusError ? ControllerHelper.CreateErrorResponse(HttpStatusCode.Conflict, "StatusError") :
                    ControllerHelper.CreateErrorResponse(HttpStatusCode.NotAcceptable, "DateError");
        }
    }
}
