using Quartz;

namespace WebPageChangeMonitor.Api;

public record JobDetailsBundle(IJobDetail Details, ITrigger Trigger);

