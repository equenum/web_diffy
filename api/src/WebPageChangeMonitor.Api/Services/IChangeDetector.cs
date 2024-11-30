using System.Threading.Tasks;
using WebPageChangeMonitor.Models.Domain;

namespace WebPageChangeMonitor.Api.Services;

public interface IChangeDetector
{
    Task ProcessAsync(TargetContext context);
}
