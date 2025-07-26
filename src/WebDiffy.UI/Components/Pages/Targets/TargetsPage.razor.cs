using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebDiffy.UI.Infrastructure;
using WebDiffy.UI.Services;
using WebPageChangeMonitor.Models.Dtos;
using SortDirection = WebPageChangeMonitor.Models.Consts.SortDirection;

namespace WebDiffy.UI.Components.Pages.Targets;

public partial class TargetsPage
{
    [Inject]
    private IDialogService DialogService { get; set; }

    [Inject]
    private ISnackbar Snackbar { get; set; }

    [Inject]
    private IResourceService ResourceService { get; set; }

    [Inject]
    private ITargetService TargetService { get; set; }

    private IEnumerable<ResourceDto> Resources = [];
    private IEnumerable<TargetDto> Targets = [];
    private Guid? ActiveResourceId = default;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        Resources = await FetchResources();
        Targets = await FetchTargets();
    }

    private async Task AddTargetAsync()
    {
        if (ActiveResourceId.HasValue)
        {
            var parameters = new DialogParameters<CreateTargetDialog>
            {
                { x => x.ResourceId, ActiveResourceId.Value }
            };

            var dialog = await DialogService.ShowAsync<CreateTargetDialog>(title: string.Empty, parameters);
            var dialogResult = await dialog.Result;

            if (!dialogResult.Canceled)
            {
                if (dialogResult.Data is not null)
                {
                    Targets = await FetchTargets();
                }
            } 
        }
    }

    private async Task UpdateTargetAsync(TargetDto target)
    {
        var parameters = new DialogParameters<UpdateTargetDialog>
        {
            { x => x.Target, target }
        };

        var dialog = await DialogService.ShowAsync<UpdateTargetDialog>(title: string.Empty, parameters);
        var dialogResult = await dialog.Result;

        if (!dialogResult.Canceled)
        {
            if (dialogResult.Data is not null)
            {
                Targets = await FetchTargets();
            }
        }
    }

    private async Task RemoveTargetAsync(Guid id)
    {
        if (UserAppSettings.AreDeletePopUpsEnabled)
        {
            await RemoveTargetInteractiveAsync(id);
            return;
        }

        await RemoveTargetSilentAsync(id);
    }

    private async Task RemoveTargetInteractiveAsync(Guid id)
    {
        var parameters = new DialogParameters<RemoveTargetDialog>
        {
            { x => x.TargetId, id }
        };

        var dialog = await DialogService.ShowAsync<RemoveTargetDialog>(title: string.Empty, parameters);
        var dialogResult = await dialog.Result;

        if (!dialogResult.Canceled)
        {
            if (dialogResult.Data is bool isSuccess && isSuccess is true)
            {
                Targets = await FetchTargets();
            }
        }
    }

    private async Task RemoveTargetSilentAsync(Guid id)
    {
        var isSuccess = false;

        try
        {
            await TargetService.RemoveAsync(id);
            isSuccess = true;

            Snackbar.Add("Target deleted", Severity.Success);
        }
        catch
        {
            Snackbar.Add("Operation 'Delete Target' failed", Severity.Error);
        }

        if (isSuccess)
        {
            Targets = await FetchTargets();
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
            count: int.MaxValue, sortDirection: SortDirection.Desc, sortBy: "DisplayName");

        return targetResponse.Targets;
    }

    private void HandlePanelMouseOver(Guid activeResourceId)
    {
        ActiveResourceId = activeResourceId;
    }
}
