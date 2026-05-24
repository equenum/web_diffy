using System;

namespace WebPageChangeMonitor.Api.Exceptions;

public class HtmlParsingException : Exception
{
    public HtmlParsingException() { }

    public HtmlParsingException(string message)
        : base(message)
    { }

    public HtmlParsingException(string message, Exception inner)
        : base(message, inner)
    { }
}
