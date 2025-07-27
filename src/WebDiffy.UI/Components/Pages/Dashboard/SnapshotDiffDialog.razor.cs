using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebPageChangeMonitor.Models.Dtos;

namespace WebDiffy.UI.Components.Pages.Dashboard;

public partial class SnapshotDiffDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }

    [Parameter]
    public TargetDto Target { get; set; }

    [Parameter]
    public TargetSnapshotDto Snapshot { get; set; }

    [Parameter]
    public string StatusMessage { get; set; }

    private bool IgnoreCase { get; set; } = false;
    private bool IgnoreWhiteSpace { get; set; } = false;

    private SideBySideDiffBuilder DiffBuilder;
    private SideBySideDiffModel DiffModel;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        DiffBuilder = new SideBySideDiffBuilder(new Differ());
        DiffModel = DiffBuilder.BuildDiffModel(Snapshot.Value, Snapshot.NewValue, IgnoreWhiteSpace, IgnoreCase);
    }

    private static string GetLineClass(ChangeType changeType)
    {
        return changeType switch
        {
            ChangeType.Inserted => "inserted",
            ChangeType.Deleted => "deleted",
            ChangeType.Modified => "modified",
            _ => "unchanged"
        };
    }

    private void UpdateIgnoreCase(bool value)
    {
        IgnoreCase = value;
        RebuildDiffModel();
    }

    private void UpdateIgnoreWhiteSpace(bool value)
    {
        IgnoreWhiteSpace = value;
        RebuildDiffModel();
    }

    private void RebuildDiffModel()
    {
        DiffModel = DiffBuilder.BuildDiffModel(Snapshot.Value, Snapshot.NewValue, IgnoreWhiteSpace, IgnoreCase);
        StateHasChanged();
    }

    private void Close() => MudDialog.Cancel();

    private void CloseAll() => MudDialog.CancelAll();
}
