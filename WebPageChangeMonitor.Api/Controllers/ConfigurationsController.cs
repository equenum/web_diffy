using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebPageChangeMonitor.Models.Options;

namespace WebPageChangeMonitor.Api;

[ApiController]
[Route("api/public/configs")]
public class ConfigurationsController : ControllerBase
{
    private readonly ChangeMonitorOptions _options;

    public ConfigurationsController(IOptions<ChangeMonitorOptions> options)
    {
        _options = options.Value;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_options);
    }

    [HttpGet("resources")]
    public IActionResult GetResources()
    {
        // todo map to resource dtos
        return Ok(_options.Resources);
    }

    [HttpGet("targets")]
    public IActionResult GetTargets(string resourceId)
    {
        // todo map to resource dto
        var resource = _options.Resources.FirstOrDefault(resource => resource.Id == resourceId);
        if (resource is null)
        {
            return NotFound();
        }

        // todo map to target dtos
        return Ok(resource.Targets);
    }
}
