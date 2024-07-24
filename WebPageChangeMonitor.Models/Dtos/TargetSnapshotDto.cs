using System;

namespace WebPageChangeMonitor.Models.Dtos;

public class TargetSnapshotDto
{
    public Guid Id { get; set; }
    public string Value { get; set; }
    public bool IsExpectedValue { get; set; }
    public bool IsChangeDetected { get; set; }
    public DateTime CreatedAt { get; set; }
}
