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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="questionId"></param>
        /// <param name="question"></param>
        /// <returns></returns>
        /// <response code="401">User is not authorized to update the question.</response>
        /// <response code="406">Question data is not acceptable.</response>
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.NotAcceptable)]
        [HttpPost]
        [Route("/Admin/Quizzes/{id}/Questions/{questionId}")]
        public async Task<JsonResult> PostUpdateQuestion(int id, int questionId, [FromBody] Question question)
        {
            var result = await questionManager.UpdateQuestion(Token.UserId, id, questionId, question);

            switch (result.Status)
            {
                case SaveQuizResultStatus.NotAuthorized:
                    return ControllerHelper.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");

                case SaveQuizResultStatus.GeneralError:
                    return ControllerHelper.CreateErrorResponse(HttpStatusCode.NotAcceptable, "GeneralError");

                default:
                    return ControllerHelper.CreateResponse(result.Result);
            }
        }
    }
}
