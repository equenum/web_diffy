using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebDiffy.UI.Services;
using WebPageChangeMonitor.Models.Dtos;
using SortDirection = WebPageChangeMonitor.Models.Consts.SortDirection;

namespace WebDiffy.UI.Components.Pages.Dashboard;

public partial class DashboardPage
{
    [Inject]
    private IResourceService ResourceService { get; set; }

    [Inject]
    private ITargetService TargetService { get; set; }

    [Inject]
    private ITargetSnapshotService TargetSnapshotService { get; set; }

    [Inject]
    private IDialogService DialogService { get; set; }

    private const int TargetSnapshotMaxCount = 20;

    private IEnumerable<ResourceDto> Resources = [];
    private Dictionary<Guid, List<TargetDto>> TargetsByResourceId = [];
    private Dictionary<Guid, List<TargetSnapshotDto>> SnapshotsByTargetId = [];

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

    private async Task<List<TargetSnapshotDto>> FetchTargetSnapshots(Guid targetId, int maxCount = TargetSnapshotMaxCount)
    {
        var targetResponse = await TargetSnapshotService.GetByTargetAsync(
            targetId, count: maxCount, sortDirection: SortDirection.Desc, sortBy: "CreatedAt");

        return [.. targetResponse.Snapshots];
    }

    private async Task OpenSnapshotHistoryAsync(TargetDto target)
    {
        var snapshots = await FetchTargetSnapshots(target.Id, int.MaxValue);
        var parameters = new DialogParameters<SnapshotHistoryDialog>
        {
            { x => x.Target, target },
            { x => x.TargetSnapshots, snapshots }
        };

        var options = new DialogOptions()
        {
            MaxWidth = MaxWidth.Large,
            FullWidth = true
        };

        var dialog = await DialogService.ShowAsync<SnapshotHistoryDialog>(title: string.Empty, parameters, options);
    }
}
