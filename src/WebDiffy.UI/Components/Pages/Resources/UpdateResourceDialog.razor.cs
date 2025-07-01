using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebDiffy.UI.Infrastructure.Helpers;
using WebDiffy.UI.Services;
using WebPageChangeMonitor.Models.Dtos;

namespace WebDiffy.UI.Components.Pages.Resources;

public partial class UpdateResourceDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }

    [Inject]
    private ISnackbar Snackbar { get; set; }

    [Inject]
    private IResourceService ResourceService { get; set; }

    [Parameter]
    public ResourceDto Resource { get; set; }

    private ResourceDto CopiedResource;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        CopiedResource = ObjectCopyHelper.DeepCopy(Resource);
    }

    private void Cancel() => MudDialog.Cancel();

    private async Task UpdateResourceAsync()
    {
        Guid? updatedResourceId = null;

        try
        {
            var updatedResource = await ResourceService.UpdateAsync(CopiedResource);
            updatedResourceId = updatedResource.Id;

            Snackbar.Add("Resource updated", Severity.Success);
        }
        catch
        {
            Snackbar.Add("Operation 'Update Resource' failed", Severity.Error);
        }

        MudDialog.Close(DialogResult.Ok(updatedResourceId));
    }
}
