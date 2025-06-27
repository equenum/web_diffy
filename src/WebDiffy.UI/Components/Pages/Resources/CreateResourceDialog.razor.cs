using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebDiffy.UI.Services;
using WebPageChangeMonitor.Models.Dtos;

namespace WebDiffy.UI.Components.Pages.Resources;

public partial class CreateResourceDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }

    [Inject]
    private ISnackbar Snackbar { get; set; }

    [Inject]
    private IResourceService ResourceService { get; set; }

    private readonly ResourceDto Resource = new();

    private void Cancel() => MudDialog.Cancel();

    private async Task AddResourceAsync()
    {
        Guid? createdResourceId = null;

        try
        {
            var createdResource = await ResourceService.CreateAsync(Resource);
            createdResourceId = createdResource.Id;

            Snackbar.Add("Resource added", Severity.Success);
        }
        catch
        {
            Snackbar.Add("Operation 'Add Resource' failed", Severity.Error);
        }

        MudDialog.Close(DialogResult.Ok(createdResourceId));
    }
}
