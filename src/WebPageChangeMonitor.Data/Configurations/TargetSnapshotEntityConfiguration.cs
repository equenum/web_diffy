using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebPageChangeMonitor.Models.Entities;

namespace WebPageChangeMonitor.Data.Configurations;

public class TargetSnapshotEntityConfiguration : IEntityTypeConfiguration<TargetSnapshotEntity>
{
    public void Configure(EntityTypeBuilder<TargetSnapshotEntity> builder)
    {
        builder.HasKey(m => m.Id);
        builder.ToTable("target_snapshots");

        builder.Property(m => m.Outcome).HasConversion<string>();

        builder.HasOne(m => m.Target);
    }
}
