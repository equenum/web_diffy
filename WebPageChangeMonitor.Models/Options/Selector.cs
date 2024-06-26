namespace WebPageChangeMonitor.Models.Options;

public class Selector
{
    // required
    public SelectorType Type { get; set; }

    // required, is not null, empty or whitespace
    public string Value { get; set; }
}
