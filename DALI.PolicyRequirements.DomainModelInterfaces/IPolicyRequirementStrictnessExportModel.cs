using System;
namespace DALI.PolicyRequirements.DomainModelInterfaces
{
    public interface IPolicyRequirementSeverityExportModel
    {
        string ShortName { get; set; }
        int Id { get; set; }
        string Description { get; set; }
    }
}
