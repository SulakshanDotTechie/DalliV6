using System;
namespace DALI.ExportEngine.Models
{
    public interface IPolicyRequirementLevelPropertyExportModel
    {
        int Sequence { get; set; }
        int Id { get; set; }
        int LevelId { get; set; }
        string Description { get; set; }
    }
}
