using System;

namespace WebPageChangeMonitor.Models.Dtos;

public class ResourceDto
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
}
