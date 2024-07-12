using System;
using System.Linq;
using HtmlAgilityPack;
using WebPageChangeMonitor.Models.Change;

namespace WebPageChangeMonitor.Services.Parsers;

public class HtmlParser : IHtmlParser
{
    public string GetNodeInnerText(string html, TargetContext context)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(html, nameof(html));
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        var document = new HtmlDocument();
        document.LoadHtml(html);

        // XPath: 
        //  - https://developer.mozilla.org/en-US/docs/Web/XPath
        //  - https://devhints.io/xpath

        // var xPath = "//span[contains(@class, 'p-name vcard-fullname d-block overflow-hidden')]";
        var xPath = $"//{context.HtmlTag}[contains(@{context.Selector.Type}, '{context.Selector.Value}')]".ToLowerInvariant();
        var targetNode = document.DocumentNode
            .SelectNodes(xPath)
            .FirstOrDefault();

        return targetNode.InnerText;
    }
}
