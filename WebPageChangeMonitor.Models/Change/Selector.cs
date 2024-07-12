using WebPageChangeMonitor.Models.Consts;

namespace WebPageChangeMonitor.Models.Change;

public class Selector
{
    // required
    public SelectorType Type { get; set; }

    // required, is not null, empty or whitespace
    public string Value { get; set; }
}
