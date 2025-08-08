using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebDiffy.UI.Infrastructure.UserSettings;
using WebDiffy.UI.Services;
using WebPageChangeMonitor.Models.Dtos;
using SortDirection = WebPageChangeMonitor.Models.Consts.SortDirection;

namespace WebDiffy.UI.Components.Pages.Resources;

public partial class ResourcesPage
{
    [Inject]
    private IDialogService DialogService { get; set; }

    [Inject]
    private ISnackbar Snackbar { get; set; }

    [Inject]
    private IResourceService ResourceService { get; set; }

    private IEnumerable<ResourceDto> Resources = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        Resources = await FetchResources();
    }

    private async Task AddResourceAsync()
    {
        var dialog = await DialogService.ShowAsync<CreateResourceDialog>();
        var dialogResult = await dialog.Result;

        if (!dialogResult.Canceled)
        {
            if (dialogResult.Data is not null)
            {
                Resources = await FetchResources();
            }
        }
    }

    private async Task UpdateResourceAsync(ResourceDto resource)
    {
        var parameters = new DialogParameters<UpdateResourceDialog>
        {
            { x => x.Resource, resource }
        };

        var dialog = await DialogService.ShowAsync<UpdateResourceDialog>(title: string.Empty, parameters);
        var dialogResult = await dialog.Result;

        if (!dialogResult.Canceled)
        {
            if (dialogResult.Data is not null)
            {
                Resources = await FetchResources();
            }
        }
    }

    private async Task RemoveResourceAsync(Guid id)
    {
        if (UserAppSettings.AreDeletePopUpsEnabled)
        {
            await RemoveResourceInteractiveAsync(id);
            return;
        }

        await RemoveResourceSilentAsync(id);
    }

    private async Task RemoveResourceInteractiveAsync(Guid id)
    { 
        var parameters = new DialogParameters<RemoveResourceDialog>
        {
            { x => x.ResourceId, id }
        };

        var dialog = await DialogService.ShowAsync<RemoveResourceDialog>(title: string.Empty, parameters);
        var dialogResult = await dialog.Result;

        if (!dialogResult.Canceled)
        {
            if (dialogResult.Data is bool isSuccess && isSuccess is true)
            {
                Resources = await FetchResources();
            }
        }
    }

    private async Task RemoveResourceSilentAsync(Guid id)
    {
        var isSuccess = false;

        try
        {
            await ResourceService.RemoveAsync(id);
            isSuccess = true;

            Snackbar.Add("Resource deleted", Severity.Success);
        }
        catch
        {
            Snackbar.Add("Operation 'Delete Resource' failed", Severity.Error);
        }
        
        if (isSuccess)
        {
            Resources = await FetchResources();
        }
    }

    private async Task<IEnumerable<ResourceDto>> FetchResources()
    {
        var resourceResponse = await ResourceService.GetAsync(
            count: int.MaxValue, sortDirection: SortDirection.Asc, sortBy: "DisplayName");

        return resourceResponse.Resources;
    }
}
