using Riok.Mapperly.Abstractions;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Models.Dtos;
using WebPageChangeMonitor.Models.Entities;

namespace WebPageChangeMonitor.Api.Infrastructure.Mappers;

[Mapper]
public static partial class TargetMapper
{
    public static partial TargetEntity ToTargetEntity(this Target target);
    public static partial TargetDto ToTargetDto(this TargetEntity targetEntity);
}
