using Quartz;

namespace WebPageChangeMonitor.Api.Models.Jobs;

public record JobDetailsBundle(IJobDetail Details, ITrigger Trigger, string JobTargetName);
