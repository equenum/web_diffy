using Prometheus;

namespace WebPageChangeMonitor.Common.Stats;

public static class MonitorMetrics
{
    public static readonly Gauge TargetsGauge =
        Metrics.CreateGauge(
            "web_diffy_targets_gauge",
            "Number of existing targets.",
            labelNames:
            [
                "status",
                "change-type"
            ]
        );
}
