using Quartz;

namespace WebPageChangeMonitor.Api.Infrastructure;

public record JobDetailsBundle(IJobDetail Details, ITrigger Trigger, string JobTargetName);
