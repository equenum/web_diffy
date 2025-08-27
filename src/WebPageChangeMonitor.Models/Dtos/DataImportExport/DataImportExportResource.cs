using System.Collections.Generic;

namespace WebPageChangeMonitor.Models.Dtos.DataImportExport;

public class DataImportExportResource
{
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public List<DataImportExportTarget> Targets { get; set; }
}
