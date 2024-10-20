using System;
using System.Runtime.Serialization;
namespace DALI.PolicyRequirements.DomainModels
{
    [KnownType(typeof(PolicyRequirementKeywordModel))]
    public class PolicyRequirementKeywordModel : IPolicyRequirementKeywordModel
    {
        public int Id { get; set; }

        public int VersionId { get; set; }

        public string Description { get; set; }

        public bool Active { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }
    }
}
