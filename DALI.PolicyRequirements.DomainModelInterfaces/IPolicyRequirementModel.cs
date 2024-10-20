using System;

namespace DALI.PolicyRequirements.DomainModels
{
    public interface IPolicyRequirementModel : IVersionedModel<int>
    {
        int? GroupIndex { get; set; }
        int? OrderIndex { get; set; }
        string Owner { get; set; }
        string DescriptionAsHtml { get; set; }
        string SemiPath { get; }
        string SemiPathKey { get; }
        string Path { get; }
        Guid? UniqueID { get; set; }

        int HasAttachments { get; set; }
        int HasSourceReferences { get; set; }
    }
}