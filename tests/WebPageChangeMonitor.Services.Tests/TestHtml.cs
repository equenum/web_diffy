namespace WebPageChangeMonitor.Services.Tests;

public static class TestHtml
{
    public static class SelectorValues
    {
        public const string Id = "test-id";
        public const string IdUpperCase = "TEST-ID";
        public const string IdMixedCase = "Test-Id";

        public const string Class = "test-class";
        public const string ClassUpperCase = "TEST-CLASS";
        public const string ClassMixedCase = "Test-Class";

        public const string XPathRelative = "//*[@id=\"counter-p\"]";
        public const string XPathFull = "/html/body/div/main/article/p";
    }

    public static class InnerTexsts
    {
        public const string Test = "TEST";
        public const string Counter = "Current count: 3";
    }

    public const string Valid = 
    """
    <!DOCTYPE html>
    <html>

    <head>
        <title>TEST</title>
    </head>

    <body>
        <div id="test-id" class="test-class">TEST</div>
    </body>

    </html>
    """;
    
    public const string ValidXPath = 
    """
    <!DOCTYPE html>
    <html>
    <body>
        <div class="page">
            <main>
                <article class="content px-4">
                    <h1 id="counter-title" class="counter-title-class">Counter</h1>
                    <p id="counter-p" class="counter-p-class" role="status">Current count: 3</p>
                    <button class="btn btn-primary">Click me</button>
                </article>
            </main>
        </div>
    </body>
    </html>
    """;
}
