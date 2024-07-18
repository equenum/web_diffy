using System.ComponentModel.DataAnnotations;

namespace WebPageChangeMonitor.Api.Requests;

public class CreateRecourseRequest
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(20)]
    public string DisplayName { get; set; }

    [StringLength(50, MinimumLength = 1)]
    public string Description { get; set; }
}
