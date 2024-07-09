using System;
using System.Linq;
using HtmlAgilityPack;

namespace WebPageChangeMonitor.Services.Parsers;

public class HtmlParser : IHtmlParser
{
    public string GetNodeInnerText(string html)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(html, nameof(html));

        var document = new HtmlDocument();
        document.LoadHtml(html);

        // XPath: 
        //  - https://developer.mozilla.org/en-US/docs/Web/XPath
        //  - https://devhints.io/xpath

        var xPath = "//span[contains(@class, 'p-name vcard-fullname d-block overflow-hidden')]";
        var targetNode = document.DocumentNode
            .SelectNodes(xPath)
            .FirstOrDefault();

        return targetNode.InnerText;
    }
}
