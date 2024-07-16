using WebPageChangeMonitor.Models.Consts;

namespace WebPageChangeMonitor.Models.Change;

public class TargetContext
{
    public string Url { get; set; }
    public ChangeType ChangeType { get; set; }
    public string HtmlTag { get; set; }
    public SelectorType SelectorType { get; set; }
    public string SelectorValue { get; set; }
    public string ExpectedValue { get; set; } 
}
