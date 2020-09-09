using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OptionsController : ControllerBase
    {
        [HttpGet]
        [Route("Quiz/{id}")]
        public IEnumerable<Option> Get(int id)
        {
            var list = new List<Option>();

            list.Add(new Option
            {
                Id = 2,
                Body = "This is an option"
            });

            list.Add(new Option
            {
                Id = 3,
                Body = "This is another option, which is the correct one."
            });

            list.Add(new Option
            {
                Id = 4,
                Body = "This is another option"
            });

            list.Add(new Option
            {
                Id = 5,
                Body = "This is yet another option"
            });

            list.Add(new Option
            {
                Id = 8,
                Body = "This is an option. One of the correct ones."
            });

            list.Add(new Option
            {
                Id = 9,
                Body = "This is not the correct one."
            });

            list.Add(new Option
            {
                Id = 10,
                Body = "This is the correct one. Please select this one."
            });

            list.Add(new Option
            {
                Id = 11,
                Body = "This is not at all the correct option."
            });

            return list;
        }
    }
}
