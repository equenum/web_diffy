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
[Route("api/public/resources")]
public class ResourcesController : ControllerBase
{
    private readonly ILogger<ResourcesController> _logger;
    private readonly ChangeMonitorOptions _options;
    private readonly IResourceService _service;
    

    public ResourcesController(
        ILogger<ResourcesController> logger,
        IOptions<ChangeMonitorOptions> options,
        IResourceService service)
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
            var response = await _service.GetAsync(page, count ?? _options.DefaultResourcePageSize);
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
            var resource = await _service.GetAsync(id);
            return Ok(resource);
        }
        catch (ResourceNotFoundException)
        {
            _logger.LogError("Resource not found: {Id}", id);
            return NotFound($"Resource not found: {id}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateResourceRequest request)
    {
        var resource = await _service.CreateAsync(request);

        return CreatedAtAction(actionName: nameof(GetById),
            routeValues: new { id = resource.Id },
            value: resource);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] Resource resource)
    {
        try
        {
            var updatedResource = await _service.UpdateAsync(resource);

            return CreatedAtAction(actionName: nameof(GetById),
                routeValues: new { id = updatedResource.Id },
                value: updatedResource);
        }
        catch (ResourceNotFoundException)
        {
            _logger.LogError("Resource not found: {Id}", resource.Id);
            return NotFound($"Resource not found: {resource.Id}");
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
        catch (ResourceNotFoundException)
        {
            _logger.LogError("Resource not found: {Id}", id);
            return NotFound($"Resource not found: {id}");
        }
    }
}
