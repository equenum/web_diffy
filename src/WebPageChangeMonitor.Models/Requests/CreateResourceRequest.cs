using System.ComponentModel.DataAnnotations;

namespace WebPageChangeMonitor.Models.Requests;

public class CreateResourceRequest
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(20)]
    public string DisplayName { get; set; }

    [StringLength(50, MinimumLength = 1)]
    public string Description { get; set; }
}
