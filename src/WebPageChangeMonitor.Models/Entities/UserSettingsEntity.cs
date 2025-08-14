using System;

namespace WebPageChangeMonitor.Models.Entities;

public class UserSettingsEntity
{
    public Guid Id { get; set; }

    public string Value { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
