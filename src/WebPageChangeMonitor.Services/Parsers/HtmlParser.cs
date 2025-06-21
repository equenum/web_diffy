using System;
using System.Linq;
using HtmlAgilityPack;
using WebPageChangeMonitor.Models.Domain;

namespace WebPageChangeMonitor.Services.Parsers;

public class HtmlParser : IHtmlParser
{
    public string GetNodeInnerText(string html, TargetContext context)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            throw new ArgumentException("Unexpected target html value.", nameof(html));
        }

        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (string.IsNullOrWhiteSpace(context.SelectorValue))
        {
            throw new InvalidOperationException("Target context selector value not specified.");    
        }

        if (string.IsNullOrWhiteSpace(context.HtmlTag))
        {
            throw new InvalidOperationException("Target html tag not specified.");    
        }

        var document = new HtmlDocument();
        document.LoadHtml(html);

        var xPath = $"//{context.HtmlTag}[contains(@{context.SelectorType}, '{context.SelectorValue}')]".ToLowerInvariant();
        var targetNodes = document.DocumentNode.SelectNodes(xPath);

        if (targetNodes is null || targetNodes.Count == 0)
        {
            throw new InvalidOperationException("Target HTML node not found.");
        }

        return targetNodes.First().InnerText;
    }
}
