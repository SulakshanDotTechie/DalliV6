using System;
namespace DALI.PolicyRequirements.DomainModels
{
    public interface IPolicyRequirementSeverityModel : IVersionedModel<int>
    {
        string ShortName { get; set; }
        string Clarification { get; set; }
        bool IsAssigned { get; set; }
        int PolicyRequirementId { get; set; }
    }
}
