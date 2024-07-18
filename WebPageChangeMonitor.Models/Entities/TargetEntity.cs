using System;
using System.Collections.Generic;
using WebPageChangeMonitor.Models.Consts;

namespace WebPageChangeMonitor.Models.Entities;

public class TargetEntity
{
    public Guid Id { get; set; }

    public string DisplayName { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public string CronSchedule { get; set; }
    public ChangeType ChangeType { get; set; }
    public string HtmlTag { get; set; }
    public SelectorType SelectorType { get; set; }
    public string SelectorValue { get; set; }
    public string ExpectedValue { get; set; } 
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Guid ResourceId { get; set; }
    public virtual ResourceEntity Resource { get; set; }

    public Guid SnapshotId { get; set; }
    public virtual IEnumerable<TargetSnapshotEntity> Snapshots { get; set; }
}
