namespace WebPageChangeMonitor.Models.Logging;

public static class LogErrorCodes
{
    public static class Detection
    {
        public const int Failed = 11;
        public const int FetchFailed = 12;
    }

    public static class Target
    {
        public const int InvalidQuery = 21;
        public const int NotFound = 22;
        public const int DeleteFailed = 23;
    }

    public static class Resource
    {
        public const int InvalidQuery = 31;
        public const int NotFound = 32;
    }

    public static class Snapshot
    {
        public const int InvalidQuery = 41;
        public const int NotFound = 42;
        public const int DeleteFailed = 43;
    }

    public static class UserSettings
    {
        public const int NotFound = 51;
    }

    public static class Job
    {
        public const int TriggerNotFound = 61;
    }

    public static class Notifications
    {
        public const int SnapshotFailed = 71;
        public const int ValueFailed = 72;
    }
}
