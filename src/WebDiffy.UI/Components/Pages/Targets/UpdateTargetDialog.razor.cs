using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebDiffy.UI.Infrastructure.Helpers;
using WebDiffy.UI.Services;
using WebPageChangeMonitor.Models.Consts;
using WebPageChangeMonitor.Models.Dtos;

namespace WebDiffy.UI.Components.Pages.Targets;

public partial class UpdateTargetDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }

    [Inject]
    private ISnackbar Snackbar { get; set; }

    [Inject]
    private ITargetService TargetService { get; set; }

    [Parameter]
    public TargetDto Target { get; set; }

    private TargetDto CopiedTarget;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        CopiedTarget = ObjectCopyHelper.DeepCopy(Target);
    }

    private void Cancel() => MudDialog.Cancel();

    private async Task UpdateTargetAsync()
    {
        await Form.Validate();
        if (!IsFormValid)
        {
            return;
        }
        
        Guid? updatedTargetId = null;

        try
        {
            var updatedTarget = await TargetService.UpdateAsync(CopiedTarget);
            updatedTargetId = updatedTarget.Id;

            Snackbar.Add("Target updated", Severity.Success);
        }
        catch
        {
            Snackbar.Add("Operation 'Update Target' failed", Severity.Error);
        }

        MudDialog.Close(DialogResult.Ok(updatedTargetId));
    }

    private void SetChangeType(string value)
    {
        CopiedTarget.ChangeType = EnumHelper.GetEnumValue<ChangeType>(value);
    } 

    private void SetSelectorType(string value)
    {
        CopiedTarget.SelectorType = EnumHelper.GetEnumValue<SelectorType>(value);
    }
}
