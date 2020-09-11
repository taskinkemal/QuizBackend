using Models.DbModels;
using Microsoft.AspNetCore.Mvc;
using WebCommon.BaseControllers;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnswersController : AuthController
    {
        [HttpPost]
        public ActionResult<bool> Post([FromBody] Answer answer)
        {

            return true;
        }
    }
}
