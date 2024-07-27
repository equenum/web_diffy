using System;

namespace WebPageChangeMonitor.Models.Options;

public class JobRetryOptions
{
    public int MaxAttempts { get; set; }
    public TimeSpan Delay { get; init; }
}
