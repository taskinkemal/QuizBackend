using System;
using System.Collections.Generic;
using Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnswersController : ControllerBase
    {
        [HttpPost]
        public ActionResult<bool> Post([FromBody] Answer answer)
        {

            return true;
        }
    }
}
