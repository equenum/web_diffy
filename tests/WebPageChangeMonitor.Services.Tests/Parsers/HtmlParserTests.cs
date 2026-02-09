using System;
using FluentAssertions;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Services.Parsers;

namespace WebPageChangeMonitor.Services.Tests.Parsers;

public class HtmlParserTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void GetNodeInnerText_InvalidHtml_ThrowsException(string html)
    {
        // Arrange 
        var context = new TargetContext()
        {
            SelectorType = SelectorType.Id,
            SelectorValue = "test"
        };

        // Act
        var action = () => new HtmlParser().GetNodeInnerText(html, context);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("Unexpected target html value. (Parameter 'html')");
    }

    [Fact]
    public void GetNodeInnerText_NullContext_ThrowsException()
    {
        // Act
        var action = () => new HtmlParser().GetNodeInnerText("html", null);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'context')");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void GetNodeInnerText_InvalidSelectorValue_ThrowsException(string selectorValue)
    {
        // Arrange 
        var context = new TargetContext()
        {
            SelectorType = SelectorType.Id,
            SelectorValue = selectorValue
        };

        // Act
        var action = () => new HtmlParser().GetNodeInnerText("html", context);

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Target selector value not specified, type: Id.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void GetNodeInnerText_InvalidHtmlTag_ThrowsException(string htmlTag)
    {
        // Arrange 
        var context = new TargetContext()
        {
            SelectorType = SelectorType.Id,
            SelectorValue = TestHtml.SelectorValues.Id,
            HtmlTag = htmlTag
        };

        // Act
        var action = () => new HtmlParser().GetNodeInnerText("html", context);

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Target html tag not specified.");
    }

    [Fact]
    public void GetNodeInnerText_NodeNotFound_ThrowsException()
    {
        // Arrange 
        var context = new TargetContext()
        {
            SelectorType = SelectorType.Id,
            SelectorValue = "not-found",
            HtmlTag = "div"
        };

        // Act
        var action = () => new HtmlParser().GetNodeInnerText(TestHtml.Valid, context);

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Target HTML node not found.");
    }

    [Theory]
    [InlineData(SelectorType.Id, TestHtml.SelectorValues.Id)]
    [InlineData(SelectorType.Class, TestHtml.SelectorValues.Class)]
    public void GetNodeInnerText_ValidArguments_ReturnsExpectedValue(
        SelectorType selectorType, string selectorValue)
    {
        // Arrange 
        var context = new TargetContext()
        {
            SelectorType = selectorType,
            SelectorValue = selectorValue,
            HtmlTag = "div"
        };

        // Act
        var innerText = new HtmlParser().GetNodeInnerText(TestHtml.Valid, context);

        // Assert
        innerText.Should().NotBeNull();
        innerText.Should().Be(TestHtml.InnerTexsts.Test);
    }

    [Theory]
    [InlineData(SelectorType.Id, TestHtml.SelectorValues.IdUpperCase)]
    [InlineData(SelectorType.Class, TestHtml.SelectorValues.ClassUpperCase)]
    [InlineData(SelectorType.Id, TestHtml.SelectorValues.IdMixedCase)]
    [InlineData(SelectorType.Class, TestHtml.SelectorValues.ClassMixedCase)]
    public void GetNodeInnerText_DifferentSelectorValueCases_ReturnsExpectedValue(
        SelectorType selectorType, string selectorValue)
    {
        // Arrange 
        var context = new TargetContext()
        {
            SelectorType = selectorType,
            SelectorValue = selectorValue,
            HtmlTag = "div"
        };

        // Act
        var innerText = new HtmlParser().GetNodeInnerText(TestHtml.Valid, context);

        // Assert
        innerText.Should().NotBeNull();
        innerText.Should().Be(TestHtml.InnerTexsts.Test);
    }

    [Theory]
    [InlineData(TestHtml.SelectorValues.XPathRelative)]
    [InlineData(TestHtml.SelectorValues.XPathFull)]
    public void GetNodeInnerText_RelativeXPath_ReturnsExpectedValue(string selectorValue)
    {
        // Arrange 
        var context = new TargetContext()
        {
            SelectorType = SelectorType.XPath,
            SelectorValue = selectorValue
        };

        // Act
        var innerText = new HtmlParser().GetNodeInnerText(TestHtml.ValidXPath, context);

        // Assert
        innerText.Should().NotBeNull();
        innerText.Should().Be(TestHtml.InnerTexsts.Counter);
    }
}
