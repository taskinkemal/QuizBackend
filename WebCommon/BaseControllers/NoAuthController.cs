using Microsoft.AspNetCore.Mvc;
using WebCommon.Attributes;

namespace WebCommon.BaseControllers
{
    /// <inheritdoc />
    /// <summary>
    /// Sets token to null when it's valid.
    /// </summary>
    [TypeFilter(typeof(ExecutionFilterAttribute), Arguments = new object[] { false })]
    public class NoAuthController : BaseController { }
}