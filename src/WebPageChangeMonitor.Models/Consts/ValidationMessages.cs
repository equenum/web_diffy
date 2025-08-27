namespace WebPageChangeMonitor.Models.Consts;

public static class ValidationMessages
{
    private const string MaxLengthMessage = "Max. length";
    private const string MaxArrayLengthMessage = "Max. array length";
    private const string MaxValueMessage = "Max. value";

    public const string Required = "Required";
    public const string Empty = "Cannot be empty";
    public const string JsonFormat = "Invalid JSON format";
    public const string ArrayFormat = "Invalid array format";
    public const string Unique = "Unique values expected";
    public const string CronExpression = "Invalid cron expression";
    public const string Url = "Invalid url format";
    public const string UrlSchema = "Invalid url schema";

    public static string MaxLength(int length) => $"{MaxLengthMessage}: {length}";
    public static string MaxArrayLength(int length) => $"{MaxArrayLengthMessage}: {length}";
    public static string MaxValue(int maxValue) => $"{MaxValueMessage}: {maxValue}";
}
