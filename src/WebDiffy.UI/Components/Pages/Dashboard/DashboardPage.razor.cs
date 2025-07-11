using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebPageChangeMonitor.Models.Dtos;

namespace WebDiffy.UI.Components.Pages.Dashboard;

public partial class DashboardPage
{
    private IEnumerable<ResourceDto> Resources = [];
    private IEnumerable<TargetDto> Targets = [];
    private IEnumerable<TargetSnapshotDto> Snapshots = [];

    // load in data on panel opening
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var resourceId = Guid.NewGuid();
        var targetId = Guid.NewGuid();

        Resources =
        [
            new ResourceDto()
            {
                Id = resourceId,
                DisplayName = "Test Resource 1"
            },
            new ResourceDto()
            {
                Id = Guid.NewGuid(),
                DisplayName = "Test Resource 2"
            }
        ];

        Targets =
        [
            new TargetDto()
            {
                Id = Guid.NewGuid(),
                ResourceId = resourceId,
                DisplayName = "Test Target 11111111"
            },
            new TargetDto()
            {
                Id = Guid.NewGuid(),
                ResourceId = resourceId,
                DisplayName = "Test Target 22222222"
            }
        ];

        Snapshots = new List<TargetSnapshotDto>()
        {
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            new TargetSnapshotDto()
            {
                Id = Guid.NewGuid(),
                TargetId = targetId,
                IsChangeDetected = false,
                CreatedAt = DateTime.Now
            },
            new TargetSnapshotDto()
            {
                Id = Guid.NewGuid(),
                TargetId = targetId,
                IsChangeDetected = true,
                CreatedAt = DateTime.Now
            },
            new TargetSnapshotDto()
            {
                Id = Guid.NewGuid(),
                TargetId = targetId,
                IsChangeDetected = false,
                CreatedAt = DateTime.Now
            }
        };
    }
}
