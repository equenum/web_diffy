using Riok.Mapperly.Abstractions;
using WebPageChangeMonitor.Models.Domain;
using WebPageChangeMonitor.Models.Dtos;
using WebPageChangeMonitor.Models.Entities;

namespace WebPageChangeMonitor.Api.Infrastructure.Mappers;

[Mapper]
public static partial class ResourceMapper
{
    public static partial ResourceEntity ToResourceEntity(this Resource resource);
    public static partial ResourceDto ToResourceDto(this ResourceEntity resourceEntity);
}
