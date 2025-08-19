namespace WebPageChangeMonitor.Models.Consts;

public static class ValidationMessages
{
    private const string MaxLengthMessage = "Max. length";

    public const string Required = "Required";
    public const string Empty = "Cannot be empty";
    public const string CronExpression = "Invalid cron expression";
    public const string Url = "Invalid url format";
    public const string UrlSchema = "Invalid url schema";

    public static string MaxLength(int length) => $"{MaxLengthMessage}: {length}";
}
