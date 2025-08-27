using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebDiffy.UI.Services;
using WebPageChangeMonitor.Models.Consts;

namespace WebDiffy.UI.Components.Pages.Settings;

public partial class DeleteDataImportExportDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }

    [Parameter]
    public DataOperationType OperationType { get; set; }

    [Inject]
    private ISnackbar Snackbar { get; set; }

    [Inject]
    private IResourceService ResourceService { get; set; }

    private void Cancel() => MudDialog.Cancel();

    private async Task DeleteAllAsync()
    {
        if (OperationType is DataOperationType.Delete)
        {
            var isSuccess = false;

            try
            {
                var resourcesResponse = await ResourceService.GetAsync(count: int.MaxValue);

                foreach (var resource in resourcesResponse.Resources)
                {
                    await ResourceService.RemoveAsync(resource.Id);
                }

                isSuccess = true;
                Snackbar.Add("All data deleted", Severity.Success);
            }
            catch
            {
                Snackbar.Add("Operation 'Delete All Data' failed", Severity.Error);
            }

            MudDialog.Close(DialogResult.Ok(isSuccess));
        }

        if (OperationType is DataOperationType.Import)
        { 
            MudDialog.Close(DialogResult.Ok(true));
        }
    }
}
