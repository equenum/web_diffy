using Riok.Mapperly.Abstractions;
using WebPageChangeMonitor.Models.Dtos;
using WebPageChangeMonitor.Models.Entities;

namespace WebPageChangeMonitor.Api.Infrastructure.Mappers;

[Mapper]
public static partial class TargetSnapshotMapper
{
    public static partial TargetSnapshotDto ToTargetSnapshotDto(this TargetSnapshotEntity targetSnapshotEntity);
}
