using Microsoft.AspNetCore.Mvc;

namespace Cheting.Controllers
{
    [ApiController]
    [Route("api/v1/test")]
    public class TestController : ControllerBase
    {
        [HttpGet("")]
        public IActionResult Test()
        {
            return Ok("hello");
        }
    }
}