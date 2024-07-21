using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebPageChangeMonitor.Api.Controller;
using WebPageChangeMonitor.Api.Exceptions;
using WebPageChangeMonitor.Models.Options;

namespace WebPageChangeMonitor.Api;

[ApiController]
[Route("api/public/snapshots")]
public class TargetSnapshotsController : ControllerBase
{
    private readonly ILogger<TargetSnapshotsController> _logger;
    private readonly ChangeMonitorOptions _options;
    private readonly ITargetSnapshotService _service;

    public TargetSnapshotsController(
        ILogger<TargetSnapshotsController> logger,
        IOptions<ChangeMonitorOptions> options,
        ITargetSnapshotService service)
    {
        _logger = logger;
        _options = options.Value;
        _service = service;
    }

    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var targetSnapshot = await _service.GetAsync(id);
            return Ok(targetSnapshot);
        }
        catch (TargetSnapshotNotFoundException)
        {
            _logger.LogError("Target snapshot not found: {Id}", id);
            return NotFound($"Target snapshot not found: {id}");
        }
    }

    [HttpGet("target/{id:Guid}")]
    public async Task<IActionResult> GetByTargetIdAsync(Guid id, int? page, int? count)
    {
        try
        {
            var response = await _service.GetByTargetIdAsync(id, page,
                count ?? _options.DefaultTargetSnapshotPageSize);
            
            return Ok(response);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            _logger.LogError("Invalid query parameter value: {ErrorMessage}.", ex.Message);
            return BadRequest($"Invalid query parameter value: {ex.Message}.");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Remove(Guid id)
    {
        try
        {
            await _service.RemoveAsync(id);
            return NoContent();
        }
        catch (TargetSnapshotNotFoundException)
        {
            _logger.LogError("Target snapshot not found: {Id}", id);
            return NotFound($"Target snapshot not found: {id}");
        }
    }

    [HttpDelete("target/{id:Guid}")]
    public async Task<IActionResult> RemoveByTargetId(Guid id)
    {
        try
        {
            await _service.RemoveByTargetIdAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError("Failed to remove target snapshots: {Message}.", ex.Message);
            return NotFound(ex.Message);
        }
    }
}
