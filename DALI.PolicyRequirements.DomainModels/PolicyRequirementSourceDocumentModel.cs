using DALI.PolicyRequirements.DomainModelInterfaces;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DALI.PolicyRequirements.DomainModels
{
    [KnownType(typeof(PolicyRequirementSourceDocumentModel))]
    public class PolicyRequirementSourceDocumentModel : MediaModel, IPolicyRequirementSourceDocumentModel, IPolicyRequirementSourceReferenceExportModel
    {
        public int PolicyRequirementId { get; set; }
        public bool IsAssigned { get; set; }
        public bool IsOnline { get; set; }
    }
}
