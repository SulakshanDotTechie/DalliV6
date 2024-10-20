using System;
namespace DALI.PolicyRequirements.DomainModels
{
    public interface IPolicyRequirementChapterModel : IVersionedModel<int>
    {
        string ChapterNumber { get; set; }
        string Owner { get; set; }
        string OwnerEmailAddress { get; set; }
        string FullChapterDescription { get; }
        bool FetchByDefault { get; set; }
        bool IsTownSpecific { get; set; }
    }
}
