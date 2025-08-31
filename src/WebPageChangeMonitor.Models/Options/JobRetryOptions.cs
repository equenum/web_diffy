using System;

namespace WebPageChangeMonitor.Models.Options;

public class JobRetryOptions
{
    public int MaxAttempts { get; init; }
    public TimeSpan Delay { get; init; }
}
