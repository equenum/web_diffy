using System;
using WebPageChangeMonitor.Models.Consts;

namespace WebPageChangeMonitor.Models.Dtos;

public class TargetSnapshotDto
{
    public Guid Id { get; set; }
    public Guid TargetId { get; set; }
    public string Value { get; set; }
    public string NewValue { get; set; }
    public bool IsExpectedValue { get; set; }
    public bool IsChangeDetected { get; set; }
    public Outcome Outcome { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
}
