using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Dtos;

namespace WebDiffy.UI.Components.Pages.Dashboard;

public partial class SnapshotHistoryDialog
{
    [Inject]
    private IDialogService DialogService { get; set; }

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }

    [Parameter]
    public TargetDto Target { get; set; }

    [Parameter]
    public List<TargetSnapshotDto> TargetSnapshots { get; set; }

    private async Task OpenSnapshotDiffAsync(TargetSnapshotDto snapshot)
    {
        var parameters = new DialogParameters<SnapshotDiffDialog>
        {
            { x => x.Target, Target },
            { x => x.Snapshot, snapshot },
            { x => x.StatusMessage, GetStatusMessage(snapshot) }
        };

        var options = new DialogOptions()
        {
            MaxWidth = MaxWidth.Large,
            FullWidth = true,
            BackdropClick = false
        };

        var dialog = await DialogService.ShowAsync<SnapshotDiffDialog>(title: string.Empty, parameters, options);
    }

    private static string GetStatusMessage(TargetSnapshotDto snapshot)
    {
        if (snapshot.Outcome is Outcome.Failure)
        {
            return "Error";
        }

        if (snapshot.IsChangeDetected)
        {
            return snapshot.IsExpectedValue ? "Expected Value Detected" : "Change Detected";
        }

        return "No changes";
    }

    private void Close() => MudDialog.Cancel();
}
