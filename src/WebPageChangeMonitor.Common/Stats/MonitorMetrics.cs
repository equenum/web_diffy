using Prometheus;

namespace WebPageChangeMonitor.Common.Stats;

public static class MonitorMetrics
{
    public static readonly Gauge ActiveTargets =
        Metrics.CreateGauge(
            "web_diffy_active_targets_gauge",
            "Number of active target jobs."
        );

    public static readonly Counter ChangeDetectionCount =
        Metrics.CreateCounter(
            "web_diffy_change_detection_count",
            "Number of change detections.",
            new CounterConfiguration
            {
                LabelNames =
                [
                    "success",
                    "change_type"
                ] 
            }
        );

    public static readonly Histogram ChangeDetectionDuration =
        Metrics.CreateHistogram(
            "web_diffy_change_detection_duration",
            "Histogram of change detection durations in seconds.",
            new HistogramConfiguration
            {
                LabelNames =
                [
                    "success",
                    "change_type"
                ]
            }
        );

    public static readonly Counter NotificationSendCount =
        Metrics.CreateCounter(
            "web_diffy_notification_send_count",
            "Number of notification requests sent.",
            new CounterConfiguration
            {
                LabelNames =
                [
                    "success",
                    "change_type",
                    "channel"
                ] 
            }
        );

    public static readonly Histogram NotificationSendDuration =
        Metrics.CreateHistogram(
            "web_diffy_notification_send_duration",
            "Histogram of notification request durations in seconds.",
            new HistogramConfiguration
            {
                LabelNames =
                [
                    "success",
                    "change_type",
                    "channel"
                ]
            }
        );
}
