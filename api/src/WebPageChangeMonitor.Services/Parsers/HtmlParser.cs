using System;
using System.Linq;
using HtmlAgilityPack;
using WebPageChangeMonitor.Models.Domain;

namespace WebPageChangeMonitor.Services.Parsers;

public class HtmlParser : IHtmlParser
{
    public string GetNodeInnerText(string html, TargetContext context)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(html, nameof(html));
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        var document = new HtmlDocument();
        document.LoadHtml(html);

        var xPath = $"//{context.HtmlTag}[contains(@{context.SelectorType}, '{context.SelectorValue}')]".ToLowerInvariant();
        var targetNode = document.DocumentNode
            .SelectNodes(xPath)
            .FirstOrDefault();

        if (targetNode is null)
        {
            throw new InvalidOperationException("Target HTML node not found.");
        }

        return targetNode.InnerText;
    }
}
