using System;

namespace WebPageChangeMonitor.Api.Exceptions;

public class TargetNotFoundException : Exception
{
    public TargetNotFoundException() { }

    public TargetNotFoundException(string message)
        : base(message)
    { }

    public TargetNotFoundException(string message, Exception inner)
        : base(message, inner)
    { }
}
