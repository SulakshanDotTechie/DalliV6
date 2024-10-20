using System;
namespace DALI.PolicyRequirements.DomainModels
{
    public interface IPolicyRequirementChangeRequestModel : IVersionedModel<int>
    {
        int PolicyRequirementId { get; set; }
        int ActionId { get; set; }
        int StatusId { get; set; }

        bool IsApproved { get; set; }

        string UserName { get; set; }
        string Remark { get; set; }
        string ShortRemark { get; set; }
        string StatusDescription { get; set; }
        string ActionDescription { get; set; }
        string AdditionalInfo { get; set; }
        string Owner { get; set; }
        string Tooltip { get; set; }
        string RejectMessage { get; set; }
    }
}
