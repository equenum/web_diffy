using System;

namespace WebPageChangeMonitor.Models.Entities;

public class TargetSnapshotEntity
{
    public Guid Id { get; set; }

    public string Value { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Guid TargetId { get; set; }
    public virtual TargetEntity Target { get; set; }
}
