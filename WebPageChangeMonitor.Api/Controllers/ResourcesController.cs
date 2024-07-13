using Microsoft.AspNetCore.Mvc;

namespace WebPageChangeMonitor.Api.Controllers;

[ApiController]
[Route("api/public/resources")]
public class ResourcesController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Resources!");
    }
}
