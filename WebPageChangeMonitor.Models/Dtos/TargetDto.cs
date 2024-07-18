using System;
using WebPageChangeMonitor.Models.Consts;

namespace WebPageChangeMonitor.Models.Dtos;

public class TargetDto
{
    public Guid Id { get; set; }
    public Guid ResourceId { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public string CronSchedule { get; set; }
    public ChangeType ChangeType { get; set; }
    public string HtmlTag { get; set; }
    public SelectorType SelectorType { get; set; }
    public string SelectorValue { get; set; }
    public string ExpectedValue { get; set; } 
}
