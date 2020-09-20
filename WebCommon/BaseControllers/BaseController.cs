using Microsoft.AspNetCore.Mvc;
using WebCommon.Attributes;
using Models.TransferObjects;
using WebCommon.Interfaces;

namespace WebCommon.BaseControllers
{
    /// <summary>
    /// </summary>
    [Route("api/[controller]")]
    [TypeFilter(typeof(ExceptionHandlerAttribute))]
    public class BaseController : ControllerBase, IBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        public AuthToken Token { get; set; } = null;
    }
}
