using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebDiffy.UI.Services;
using WebPageChangeMonitor.Models.Dtos;

namespace WebDiffy.UI.Components.Pages.Resources;

public partial class ResourcesPage
{
    [Inject]
    private IDialogService DialogService { get; set; }

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

    private async Task<IEnumerable<ResourceDto>> FetchResources()
    {
        var resourceResponse = await ResourceService.GetAsync(count: int.MaxValue);
        return resourceResponse.Resources;
    }
}
