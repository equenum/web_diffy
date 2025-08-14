using System;

namespace WebPageChangeMonitor.Api.Exceptions;

public class UserSettingsNotFoundException : Exception
{
    public UserSettingsNotFoundException() { }

    public UserSettingsNotFoundException(string message)
        : base(message)
    { }

    public UserSettingsNotFoundException(string message, Exception inner)
        : base(message, inner)
    { }
}
