using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebDiffy.UI.Services;

namespace WebDiffy.UI.Components.Pages.Resources;

public partial class RemoveResourceDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }

    [Inject]
    private ISnackbar Snackbar { get; set; }

    [Inject]
    private IResourceService ResourceService { get; set; }

    [Parameter]
    public Guid ResourceId { get; set; }

    private void Cancel() => MudDialog.Cancel();

    private async Task RemoveResourceAsync()
    {
        var isSuccess = false;

        try
        {
            await ResourceService.RemoveAsync(ResourceId);
            isSuccess = true;

            Snackbar.Add("Resource deleted", Severity.Success);
        }
        catch
        {
            Snackbar.Add("Operation 'Delete Resource' failed", Severity.Error);
        }

        MudDialog.Close(DialogResult.Ok(isSuccess));
    }
}
