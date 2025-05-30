using System;
using System.Collections.Generic;
using WebPageChangeMonitor.Models.Dtos;

namespace WebDiffy.UI.Components.Pages.Resources;

public partial class ResourcesPage
{
    private IEnumerable<ResourceDto> _resources = [];
    
    protected override void OnInitialized()
    {
        base.OnInitialized();

        // extract into a static class
        _resources =
        [
            new()
            {
                Id = Guid.NewGuid(),
                DisplayName = "Resource 1",
                Description = "Resource 1"
            },
            new()
            {
                Id = Guid.NewGuid(),
                DisplayName = "Resource 2",
                Description = "Resource 2"
            },
            new()
            {
                Id = Guid.NewGuid(),
                DisplayName = "Resource 3",
                Description = "Resource 3"
            }
        ];
    }

}
