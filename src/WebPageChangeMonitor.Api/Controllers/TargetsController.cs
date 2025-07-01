using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebPageChangeMonitor.Api.Exceptions;
using WebPageChangeMonitor.Api.Services.Controller;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Models.Options;
using WebPageChangeMonitor.Models.Requests;

namespace WebPageChangeMonitor.Api.Controllers;

[ApiController]
[Route("api/public/targets")]
public class TargetsController : ControllerBase
{
    private readonly ILogger<TargetsController> _logger;
    private readonly ChangeMonitorOptions _options;
    private readonly ITargetService _service;

    public TargetsController(
        ILogger<TargetsController> logger,
        IOptions<ChangeMonitorOptions> options,
        ITargetService service)
    {
        _logger = logger;
        _options = options.Value;
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int? page, int? count)
    {
        try
        {
            var response = await _service.GetAsync(page, count ?? _options.DefaultTargetPageSize);
            return Ok(response);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            _logger.LogError("Invalid query parameter value: {ErrorMessage}.", ex.Message);
            return BadRequest($"Invalid query parameter value: {ex.Message}.");
        }
    }

    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var target = await _service.GetAsync(id);
            return Ok(target);
        }
        catch (TargetNotFoundException)
        {
            _logger.LogError("Target not found: {Id}", id);
            return NotFound($"Target not found: {id}");
        }
    }

    [HttpGet("resource/{id:Guid}")]
    public async Task<IActionResult> GetByResourceIdAsync(Guid id, int? page, int? count)
    {
        try
        {
            var response = await _service.GetByResourceIdAsync(id, page,
                count ?? _options.DefaultTargetPageSize);
            
            return Ok(response);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            _logger.LogError("Invalid query parameter value: {ErrorMessage}.", ex.Message);
            return BadRequest($"Invalid query parameter value: {ex.Message}.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTargetRequest request)
    {
        var target = await _service.CreateAsync(request);

        return CreatedAtAction(actionName: nameof(GetById),
            routeValues: new { id = target.Id },
            value: target);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] Target target)
    {
        try
        {
            var updatedTarget = await _service.UpdateAsync(target);

            return CreatedAtAction(actionName: nameof(GetById),
                routeValues: new { id = updatedTarget.Id },
                value: updatedTarget);
        }
        catch (ResourceNotFoundException)
        {
            _logger.LogError("Resource doesn't exist: {ResourceId}", target.ResourceId);
            return BadRequest($"Resource doesn't exist: {target.ResourceId}");
        }
        catch (TargetNotFoundException)
        {
            _logger.LogError("Target not found: {TargetId}", target.Id);
            return NotFound($"Target not found: {target.Id}");
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
        catch (TargetNotFoundException)
        {
            _logger.LogError("Target not found: {Id}", id);
            return NotFound($"Target not found: {id}");
        }
    }

    [HttpDelete("resource/{id:Guid}")]
    public async Task<IActionResult> RemoveByResourceId(Guid id)
    {
        try
        {
            await _service.RemoveByResourceIdAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError("Failed to remove targets: {Message}.", ex.Message);
            return NotFound(ex.Message);
        }
    }
}
