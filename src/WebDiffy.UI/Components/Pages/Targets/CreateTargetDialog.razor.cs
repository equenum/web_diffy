using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebDiffy.UI.Infrastructure.Helpers;
using WebDiffy.UI.Services;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Dtos;

namespace WebDiffy.UI.Components.Pages.Targets;

public partial class CreateTargetDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }

    [Inject]
    private ISnackbar Snackbar { get; set; }

    [Inject]
    private ITargetService TargetService { get; set; }

    [Parameter]
    public Guid ResourceId { get; set; }

    private readonly TargetDto Target = new();

    private void Cancel() => MudDialog.Cancel();

    private async Task AddTargetAsync()
    {
        Guid? createdTargetId = null;

        try
        {
            Target.ResourceId = ResourceId;
            
            var createdTarget = await TargetService.CreateAsync(Target);
            createdTargetId = createdTarget.Id;

            Snackbar.Add("Target added", Severity.Success);
        }
        catch
        {
            Snackbar.Add("Operation 'Add Target' failed", Severity.Error);
        }

        MudDialog.Close(DialogResult.Ok(createdTargetId));
    }

    private void SetChangeType(string value)
    {
        Target.ChangeType = EnumHelper.GetEnumValue<ChangeType>(value);
    } 

    private void SetSelectorType(string value)
    {
        Target.SelectorType = EnumHelper.GetEnumValue<SelectorType>(value);
    }
}
