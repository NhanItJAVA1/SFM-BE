using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SFM_BE.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected long GetUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(value, out var userId) ? userId : throw new UnauthorizedAccessException("User id is missing from token.");
        }
    }
}
