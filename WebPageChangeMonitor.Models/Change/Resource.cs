using System;

namespace WebPageChangeMonitor.Models.Change;

// todo implement separate models for asp.net validation and repos

// implement validation
public class Resource
{
    // required, not null, empty or whitespace
    public Guid Id { get; set; }

    // not empty or whitespace
    public string DisplayName { get; set; }

    // not empty or whitespace
    public string Description { get; set; }

    // not null or empty
    public Target[] Targets { get; set; }
}
