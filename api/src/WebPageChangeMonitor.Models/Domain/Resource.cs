using System;
using System.ComponentModel.DataAnnotations;

namespace WebPageChangeMonitor.Models.Domain;

public class Resource
{
    [Required]
    public Guid Id { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(20)]
    public string DisplayName { get; set; }

    [StringLength(50, MinimumLength = 1)]
    public string Description { get; set; }
}
