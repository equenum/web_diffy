using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using WebDiffy.UI.Services;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Dtos;

namespace WebDiffy.UI.Components.Pages.Dashboard;

public partial class DashboardPage
{
    [Inject]
    private IResourceService ResourceService { get; set; }

    [Inject]
    private ITargetService TargetService { get; set; }

    [Inject]
    private ITargetSnapshotService TargetSnapshotService { get; set; }

    private const int TargetSnapshotMaxCount = 20;

    private IEnumerable<ResourceDto> Resources = [];
    private Dictionary<Guid, List<TargetDto>> TargetsByResourceId = [];
    private Dictionary<Guid, IEnumerable<TargetSnapshotDto>> SnapshotsByTargetId = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        Resources = await FetchResources();
        var targets = await FetchTargets();

        TargetsByResourceId = targets
            .GroupBy(target => target.ResourceId)
            .ToDictionary(group => group.Key, group => group.ToList());

        foreach (var target in targets)
        {
            var snapshots = await FetchTargetSnapshots(target.Id);

            var emptySnapshotCount = TargetSnapshotMaxCount - snapshots.Count;
            for (int i = 0; i < emptySnapshotCount; i++)
            {
                snapshots.Add(null);
            }

            snapshots.Reverse();
            SnapshotsByTargetId.TryAdd(target.Id, snapshots);
        }
    }

    private async Task<IEnumerable<ResourceDto>> FetchResources()
    {
        var resourceResponse = await ResourceService.GetAsync(
            count: int.MaxValue, sortDirection: SortDirection.Asc, sortBy: "DisplayName");

        return resourceResponse.Resources;
    }

    private async Task<IEnumerable<TargetDto>> FetchTargets()
    {
        var targetResponse = await TargetService.GetAsync(
            count: int.MaxValue, sortDirection: SortDirection.Asc, sortBy: "DisplayName");

        return targetResponse.Targets;
    }

    private async Task<List<TargetSnapshotDto>> FetchTargetSnapshots(Guid targetId)
    {
        var targetResponse = await TargetSnapshotService.GetByTargetAsync(
            targetId, count: TargetSnapshotMaxCount, sortDirection: SortDirection.Desc, sortBy: "CreatedAt");

        return [.. targetResponse.Snapshots];
    }
}
