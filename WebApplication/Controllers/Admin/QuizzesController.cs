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
        /// <response code="401">User is not authorized to update the quiz.</response>
        /// <response code="406">Quiz data is not acceptable.</response>
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.NotAcceptable)]
        [HttpPost]
        [Route("{id}")]
        public async Task<JsonResult> PostUpdateQuiz(int id, [FromBody] Quiz quiz)
        {
            var result = await quizManager.UpdateQuiz(Token.UserId, id, quiz);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quiz"></param>
        /// <returns></returns>
        /// <response code="401">User is not authorized to add the quiz.</response>
        /// <response code="406">Quiz data is not acceptable.</response>
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.NotAcceptable)]
        [HttpPut]
        public async Task<JsonResult> PutInsertQuiz([FromBody] Quiz quiz)
        {
            var result = await quizManager.InsertQuiz(Token.UserId, quiz);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="401">User is not authorized to delete the quiz.</response>
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(HttpErrorMessage), (int)HttpStatusCode.Unauthorized)]
        [HttpDelete]
        [Route("{id}")]
        public async Task<JsonResult> Delete(int id)
        {
            var result = await quizManager.DeleteQuiz(Token.UserId, id);

            return result ?
                ControllerHelper.CreateResponse(result) :
                ControllerHelper.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");
        }
    }
}
