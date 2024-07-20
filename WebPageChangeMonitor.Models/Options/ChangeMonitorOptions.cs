namespace WebPageChangeMonitor.Models.Options;

public class ChangeMonitorOptions
{
    public const string SectionName = "ChangeMonitor";

    public int DefaultResourcePageSize { get; init; }
    public int DefaultTargetPageSize { get; init; }
}
