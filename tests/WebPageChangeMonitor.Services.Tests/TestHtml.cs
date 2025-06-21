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
    }

    public static class InnerTexsts
    {
        public const string Test = "TEST";
    }

    public const string Valid = @"<!DOCTYPE html><html><head><title>TEST</title></head><body><div id=""test-id"" class=""test-class"">TEST</div></body></html>";
}
