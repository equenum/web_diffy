using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebPageChangeMonitor.Data.Entities;

namespace WebPageChangeMonitor.Data.Configurations;

public class ResourceEntityConfiguration : IEntityTypeConfiguration<ResourceEntity>
{
    public void Configure(EntityTypeBuilder<ResourceEntity> builder)
    {
        builder.HasKey(m => m.Id);
        builder.ToTable("target_resources");
        builder.HasMany(m => m.Targets);
    }
}
