using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebDiffy.UI.Services;

namespace WebDiffy.UI.Components.Pages.Targets;

public partial class RemoveTargetDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }

    [Inject]
    private ISnackbar Snackbar { get; set; }

    [Inject]
    private ITargetService TargetService { get; set; }

    [Parameter]
    public Guid TargetId { get; set; }

    private void Cancel() => MudDialog.Cancel();

    private async Task RemoveTargetAsync()
    {
        var isSuccess = false;

        try
        {
            await TargetService.RemoveAsync(TargetId);
            isSuccess = true;

            Snackbar.Add("Target deleted", Severity.Success);
        }
        catch
        {
            Snackbar.Add("Operation 'Delete Target' failed", Severity.Error);
        }

        MudDialog.Close(DialogResult.Ok(isSuccess));
    }
}
