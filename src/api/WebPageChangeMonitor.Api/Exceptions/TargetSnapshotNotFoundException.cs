using System;

namespace WebPageChangeMonitor.Api.Exceptions;

public class TargetSnapshotNotFoundException : Exception
{
    public TargetSnapshotNotFoundException() { }

    public TargetSnapshotNotFoundException(string message)
        : base(message)
    { }

    public TargetSnapshotNotFoundException(string message, Exception inner)
        : base(message, inner)
    { }
}
