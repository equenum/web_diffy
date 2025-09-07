using System;
using WebPageChangeMonitor.Models.Consts;

namespace WebPageChangeMonitor.Models.Domain;

public class TargetContext
{
    public Guid Id { get; set; }
    public Guid ResourceId { get; set; }
    public string DisplayName { get; set; }
    public string Url { get; set; }
    public ChangeType ChangeType { get; set; }
    public string HtmlTag { get; set; }
    public SelectorType SelectorType { get; set; }
    public string SelectorValue { get; set; }
    public string ExpectedValue { get; set; } 
}
