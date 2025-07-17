using System;
using WebPageChangeMonitor.Models.Consts;

namespace WebPageChangeMonitor.Models.Entities;

public class TargetSnapshotEntity
{
    public Guid Id { get; set; }

    public string Value { get; set; }
    public bool IsExpectedValue { get; set; }
    public bool IsChangeDetected { get; set; }
    public DateTime CreatedAt { get; set; }
    public Outcome Outcome { get; set; }
    public string Message { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Guid TargetId { get; set; }
    public virtual TargetEntity Target { get; set; }
}
