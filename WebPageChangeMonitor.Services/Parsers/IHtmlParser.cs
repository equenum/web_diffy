using WebPageChangeMonitor.Models.Domain;

namespace WebPageChangeMonitor.Services.Parsers;

public interface IHtmlParser
{
    string GetNodeInnerText(string html, TargetContext context);
}
