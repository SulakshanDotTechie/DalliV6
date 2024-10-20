using System;
namespace DALI.PolicyRequirements.DomainModels
{
    public interface IPolicyRequirementLevelModel : IVersionedModel<int>
    {
        string Name { get; set; }
        string Position { get; set; }
    }
}
