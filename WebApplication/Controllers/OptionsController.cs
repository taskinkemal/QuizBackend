using System.Collections.Generic;
using Models.DbModels;
using Microsoft.AspNetCore.Mvc;
using WebCommon.BaseControllers;
using BusinessLayer.Interfaces;
using System.Threading.Tasks;

namespace WebApplication.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class OptionsController : AuthController
    {
        private readonly IOptionManager optionManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionManager"></param>
        public OptionsController(IOptionManager optionManager)
        {
            this.optionManager = optionManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Quiz/{id}")]
        public async Task<IEnumerable<Option>> Get(int id)
        {
            return await optionManager.GetQuizOptions(id);
        }
    }
}
