using System;
namespace DALI.PolicyRequirements.DomainModels
{
    public interface IPolicyRequirementAreaModel : IVersionedModel<int>
    {
        bool IsTownSpecific { get; set; }
        bool FetchByDefault { get; set; }
    }
}
