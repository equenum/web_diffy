using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebDiffy.UI.Services;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Dtos;
using WebPageChangeMonitor.Models.Dtos.DataImportExport;

namespace WebDiffy.UI.Components.Pages.Settings;

public partial class DataImportExportDialog
{
    private readonly JsonSerializerOptions SerializerOptions =
        new JsonSerializerOptions { WriteIndented = true };

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }

    [Parameter]
    public DataOperationType OperationType { get; set; }

    [Inject]
    private ISnackbar Snackbar { get; set; }

    [Inject]
    private IResourceService ResourceService { get; set; }

    [Inject]
    private ITargetService TargetService { get; set; }

    [Inject]
    private IDialogService DialogService { get; set; }

    private string JsonData = string.Empty;

    private void Cancel() => MudDialog.Cancel();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (OperationType is DataOperationType.Export)
        {
            var resourcesResponse = await ResourceService.GetAsync(count: int.MaxValue);
            var targetsResponse = await TargetService.GetAsync(count: int.MaxValue);

            var targetsByResourceIds = targetsResponse.Targets
                .GroupBy(target => target.ResourceId)
                .ToDictionary(group => group.Key, group => group.ToList());

            var resourceDtos = new List<DataImportExportResource>();

            foreach (var resource in resourcesResponse.Resources)
            {
                var targetDtos = targetsByResourceIds.TryGetValue(resource.Id, out var targets) ? targets : [];

                resourceDtos.Add(new DataImportExportResource()
                {
                    DisplayName = resource.DisplayName,
                    Description = resource.Description,
                    Targets =
                    [
                        .. targetDtos.Select(target =>
                        new DataImportExportTarget()
                        {
                            State = target.State,
                            DisplayName = target.DisplayName,
                            Description = target.Description,
                            Url = target.Url,
                            CronSchedule = target.CronSchedule,
                            ChangeType = target.ChangeType,
                            HtmlTag = target.HtmlTag,
                            SelectorType = target.SelectorType,
                            SelectorValue = target.SelectorValue,
                            ExpectedValue  = target.ExpectedValue
                        })
                    ]
                });
            }

            JsonData = JsonSerializer.Serialize(
                new DataImportExportDto() { Resources = resourceDtos },
                SerializerOptions
            );
        }
    }

    private async void Import()
    {
        await Form.Validate();
        if (!Form.IsValid || OperationType is not DataOperationType.Import)
        {
            return;
        }

        var parameters = new DialogParameters<DeleteDataImportExportDialog>
        {
            { x => x.OperationType, DataOperationType.Import }
        };

        var dialog = await DialogService.ShowAsync<DeleteDataImportExportDialog>(title: string.Empty, parameters);
        var dialogResult = await dialog.Result;

        if (!dialogResult.Canceled)
        {
            if (dialogResult.Data is bool isSuccess && isSuccess is true)
            {
                var isDeleteSuccess = await DeleteDataAsync();
                if (isDeleteSuccess)
                {
                    await CreateResourcesAsync();
                }
            }
        }
    }

    private async Task<bool> DeleteDataAsync()
    {
        try
        {
            var resourcesResponse = await ResourceService.GetAsync(count: int.MaxValue);

            foreach (var resource in resourcesResponse.Resources)
            {
                await ResourceService.RemoveAsync(resource.Id);
            }

            return true;
        }
        catch
        {
            Snackbar.Add("Operation 'Import Data' failed", Severity.Error);
            return false;
        }
    }

    private async Task CreateResourcesAsync()
    {
        try
        {
            var importDto = JsonSerializer.Deserialize<DataImportExportDto>(JsonData);

            foreach (var resource in importDto.Resources)
            {
                var createdResource = await ResourceService.CreateAsync(new ResourceDto()
                {
                    DisplayName = resource.DisplayName,
                    Description = resource.Description
                });

                foreach (var target in resource.Targets)
                {
                    await TargetService.CreateAsync(new TargetDto()
                    {
                        ResourceId = createdResource.Id,
                        State = target.State,
                        DisplayName = target.DisplayName,
                        Description = target.Description,
                        Url = target.Url,
                        CronSchedule = target.CronSchedule,
                        ChangeType = target.ChangeType,
                        HtmlTag = target.HtmlTag,
                        SelectorType = target.SelectorType,
                        SelectorValue = target.SelectorValue,
                        ExpectedValue = target.ExpectedValue
                    });
                }
            }

            MudDialog.Cancel();
            Snackbar.Add("Data imported", Severity.Success);
        }
        catch
        {
            Snackbar.Add("Operation 'Import Data' failed", Severity.Error);
        }
    }
}
