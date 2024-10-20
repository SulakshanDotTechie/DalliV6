using System;
namespace DALI.PolicyRequirements.DomainModelInterfaces
{
    public interface IPolicyRequirementLevelPropertyExportModel
    {
        int Sequence { get; set; }
        int Id { get; set; }
        int LevelId { get; set; }
        string Description { get; set; }
    }
}
