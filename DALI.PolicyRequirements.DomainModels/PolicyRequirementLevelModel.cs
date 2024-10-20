using System.ComponentModel.DataAnnotations;
using System;
using System.Runtime.Serialization;
using DALI.PolicyRequirements.DomainModelInterfaces;
using DALI.Topics.SharedInfrastructure;

namespace DALI.PolicyRequirements.DomainModels
{
    [KnownType(typeof(PolicyRequirementLevelModel))]
    public class PolicyRequirementLevelModel : IPolicyRequirementLevelModel, IPolicyRequirementLevelExportModel, ILevelTopicModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }

        public int VersionId { get; set; }

        public string Description { get; set; }

        public bool Active { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }
    }
}
