using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebPageChangeMonitor.Models.Entities;

namespace WebPageChangeMonitor.Data.Configurations;

public class UserSettingsEntityConfiguration : IEntityTypeConfiguration<UserSettingsEntity>
{
    public void Configure(EntityTypeBuilder<UserSettingsEntity> builder)
    {
        builder.HasKey(m => m.Id);
        builder.ToTable("user_settings");
    }
}
