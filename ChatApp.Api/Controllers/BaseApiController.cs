using ChatApp.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(AuthFilter))]
    public class BaseApiController : ControllerBase
    {
    }
}
