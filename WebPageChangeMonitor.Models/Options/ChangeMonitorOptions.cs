namespace WebPageChangeMonitor.Models.Options;

public class ChangeMonitorOptions
{
    public const string SectionName = "ChangeMonitor";

    // check if empty during startup
    public Resource[] Resources { get; set; }
}
