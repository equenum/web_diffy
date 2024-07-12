using WebPageChangeMonitor.Models.Consts;

namespace WebPageChangeMonitor.Models.Change;

public class TargetContext
{
    public string Url { get; set; }
    public TargetType Type { get; set; }
    public string HtmlTag { get; set; }
    public Selector Selector { get; set; }
    public string ExpectedValue { get; set; } 
}
