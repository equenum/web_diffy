using System;
using System.Collections.Generic;

namespace WebPageChangeMonitor.Models.Entities;

public class ResourceEntity
{
    public Guid Id { get; set; }

    public string DisplayName { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual IEnumerable<TargetEntity> Targets { get; set; }
}
