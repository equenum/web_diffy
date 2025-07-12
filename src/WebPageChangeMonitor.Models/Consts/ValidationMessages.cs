namespace WebPageChangeMonitor.Models.Consts;

// remove this class after enabling attribute based validation when dotnet 10 comes out
public static class ValidationMessages
{
    private const string MaxLengthMessage = "Max. length";

    public const string Required = "Required";
    public const string Empty = "Cannot be empty or whitespace";
    public const string CronExpression = "Invalid quartz cron expression format";
    public const string Url = "Invalid url format";
    public const string UrlSchema = "Invalid url schema";

    public static string MaxLength(int length) => $"{MaxLengthMessage}: {length}";
}
