using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DALI.PolicyRequirements.DomainModels
{
    [KnownType(typeof(PolicyRequirementAdditionalInfoModel))]
    public class PolicyRequirementAdditionalInfoModel
    {
        public int PolicyRequirementId { get; set; }
        public string PolicyRequirementDescription { get; set; }
        public int Action { get; set; }

        public int VersionId { get; set; }

        public string Description { get; set; }

        public bool InActive { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }
    }
}
