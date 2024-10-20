using System;
namespace DALI.PolicyRequirements.DomainModels
{
    public interface IPolicyRequirementLevelPropertyModel : IVersionedModel<int>
    {
        int Sequence { get; set; }
    }
}
