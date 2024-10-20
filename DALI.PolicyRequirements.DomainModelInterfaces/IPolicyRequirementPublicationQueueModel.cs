using System;
namespace DALI.PolicyRequirements.DomainModels
{
    public interface IPolicyRequirementPublicationQueueModel
    {
        int Id { get; set; }
        bool Active { get; set; }
        bool? CanPublish { get; set; }
        int VersionId { get; set; }
        DateTime? CreatedDate { get; set; }
        string Comment { get; set; }
    }
}