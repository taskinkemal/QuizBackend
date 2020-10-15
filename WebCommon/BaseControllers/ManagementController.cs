using Common;
using Microsoft.AspNetCore.Mvc;
using WebCommon.Attributes;

namespace WebCommon.BaseControllers
{
    /// <summary>
    /// Base controller for admin controllers.
    /// </summary>
    [Route("Admin/[controller]")]
    [TypeFilter(typeof(ExecutionFilterAttribute), Arguments = new object[] { AuthenticationLevel.Admin })]
    public class ManagementController : BaseController { }
}
