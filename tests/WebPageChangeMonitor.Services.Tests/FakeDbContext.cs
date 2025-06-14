using Microsoft.EntityFrameworkCore;
using WebPageChangeMonitor.Data;

namespace WebPageChangeMonitor.Services.Tests;

public class FakeDbContext
{
    public static MonitorDbContext GetInstance() => new
    (
        new DbContextOptionsBuilder<MonitorDbContext>()
            .UseInMemoryDatabase(databaseName: "ChangeMonitor")
            .Options
    );

    public static void Reset()
    {
        using (var dbContext = GetInstance())
        {
            dbContext.TargetSnapshots.RemoveRange(dbContext.TargetSnapshots);
            dbContext.Resources.RemoveRange(dbContext.Resources);
            dbContext.Targets.RemoveRange(dbContext.Targets);

            dbContext.SaveChanges();
        }
    }
}
