using System;
namespace DALI.ExportEngine.Models
{
    public interface IPolicyRequirementSeverityExportModel
    {
        string ShortName { get; set; }
        int Id { get; set; }
        string Description { get; set; }
    }
}
