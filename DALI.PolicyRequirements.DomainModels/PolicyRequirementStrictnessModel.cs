using System.ComponentModel.DataAnnotations;
using System;
using System.Runtime.Serialization;
using DALI.PolicyRequirements.DomainModelInterfaces;
using DALI.Topics.SharedInfrastructure;

namespace DALI.PolicyRequirements.DomainModels
{
    [KnownType(typeof(PolicyRequirementSeverityModel))]
    public class PolicyRequirementSeverityModel : IPolicyRequirementSeverityModel, IPolicyRequirementSeverityExportModel, ISeverityTopicModel
    {
        public int Id { get; set; }

        public int VersionId { get; set; }

        public string ShortName { get; set; }

        public string Clarification { get; set; }

        public int PolicyRequirementId { get; set; }

        public string Description { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool Active { get; set; }

        public bool IsAssigned { get; set; }

        public string AdditionalInfo { get; set; }
    }
}
