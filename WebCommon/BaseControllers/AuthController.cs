using Common;
using Microsoft.AspNetCore.Mvc;
using WebCommon.Attributes;

namespace WebCommon.BaseControllers
{
    /// <inheritdoc />
    /// <summary>
    /// Throws when token is invalid.
    /// </summary>
    [TypeFilter(typeof(ExecutionFilterAttribute), Arguments = new object[] { AuthenticationLevel.User })]
    public class AuthController : BaseController { }
}