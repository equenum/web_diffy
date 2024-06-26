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

    public IActionResult Get()
    {
        return Ok(_options);
    }
}
