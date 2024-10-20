using System;
namespace DALI.PolicyRequirements.DomainModels
{
    public interface IPolicyRequirementSubjectModel : IVersionedModel<int>
    {
        bool FetchByDefault { get; set; }
        bool IsTownSpecific { get; set; }
    }
}
