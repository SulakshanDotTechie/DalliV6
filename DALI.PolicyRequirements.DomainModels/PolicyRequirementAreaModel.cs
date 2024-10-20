using DALI.PolicyRequirements.DomainModelInterfaces;
using DALI.Topics.SharedInfrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DALI.PolicyRequirements.DomainModels
{
    [KnownType(typeof(PolicyRequirementAreaModel))]
    public class PolicyRequirementAreaModel : IPolicyRequirementAreaModel, IPolicyRequirementAreaExportModel, IAreaTopicModel
    {
        public int Id { get; set; }

        public int VersionId { get; set; }

        public string Description { get; set; }

        public bool Active { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public bool IsTownSpecific { get; set; }

        public bool FetchByDefault { get; set; }
    }
}
