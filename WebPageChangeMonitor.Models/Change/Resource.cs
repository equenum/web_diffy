namespace WebPageChangeMonitor.Models.Change;

// implement validation
public class Resource
{
    // required, not null, empty or whitespace
    public string Id { get; set; }

    // not empty or whitespace
    public string DisplayName { get; set; }

    // not empty or whitespace
    public string Description { get; set; }

    // not null or empty
    public Target[] Targets { get; set; }
}
