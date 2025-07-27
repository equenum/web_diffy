using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebPageChangeMonitor.Models.Dtos;

namespace WebDiffy.UI.Components.Pages.Dashboard;

public partial class SnapshotHistoryDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }

    [Parameter]
    public TargetDto Target { get; set; }

    [Parameter]
    public List<TargetSnapshotDto> TargetSnapshots { get; set; }

    private async Task OpenSnapshotDiffAsync(TargetSnapshotDto snapshot)
    {

    }

    private void Close() => MudDialog.Cancel();
}
