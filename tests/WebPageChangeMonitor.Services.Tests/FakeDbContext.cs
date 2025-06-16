using Microsoft.EntityFrameworkCore;
using WebPageChangeMonitor.Data;

namespace WebPageChangeMonitor.Services.Tests;

public class FakeDbContext
{
    public static MonitorDbContext GetInstance(string dbName) => new
    (
        new DbContextOptionsBuilder<MonitorDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options
    );

    public static void Reset(string dbName)
    {
        using (var dbContext = GetInstance(dbName))
        {
            dbContext.Database.EnsureDeleted();
        }
    }
}
