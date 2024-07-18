using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebPageChangeMonitor.Api.Exceptions;
using WebPageChangeMonitor.Api.Requests;
using WebPageChangeMonitor.Api.Services.Controller;
using WebPageChangeMonitor.Models.Domain;

namespace WebPageChangeMonitor.Api.Controllers;

[ApiController]
[Route("api/public/resources")]
public class ResourcesController : ControllerBase
{
    // todo extract to configs
    private const int DefaultPageSize = 10;

    private readonly ILogger<ResourcesController> _logger;
    private readonly IResourceService _service;

    public ResourcesController(
        ILogger<ResourcesController> logger,
        IResourceService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int? page, int count = DefaultPageSize)
    {
        try
        {
            var response = await _service.GetAsync(page, count);
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
    public async Task<IActionResult> Create([FromBody] CreateRecourseRequest request)
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
                routeValues: new { id = resource.Id },
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
