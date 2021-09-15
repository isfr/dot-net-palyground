using Microsoft.AspNetCore.Mvc;

namespace BankApi.WebApi.Controllers
{
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        [Route("/status")]
        public IActionResult GetStatus()
        {
            return Ok();
        }
    }
}
