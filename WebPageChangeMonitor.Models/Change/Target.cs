using System;
using WebPageChangeMonitor.Models.Consts;

namespace WebPageChangeMonitor.Models.Change;

// implement validation
public class Target
{
    // required, not null, empty or whitespace
    public Guid Id { get; set; }

    // required, not null, empty or whitespace
    public Guid ResourceId { get; set; }

    // not empty or whitespace
    public string DisplayName { get; set; }

    // not empty or whitespace
    public string Description { get; set; }

    // todo make the type Uri?
    // required, not null or whitespace
    public string Url { get; set; }

    // required, not null, empty or whitespace
    // validate the cron itself?
    // add default value
    public string CronSchedule { get; set; }

    // required, not null or whitespace
    public ChangeType ChangeType { get; set; }

    // required, not null or whitespace
    public string HtmlTag { get; set; }

    // optional
    public SelectorType SelectorType { get; set; }

    // required, is not null, empty or whitespace
    public string SelectorValue { get; set; }

    // only supposed to have value when target type is ValueCheck
    // not null or whitespace
    public string ExpectedValue { get; set; } 
}
