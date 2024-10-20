using DALI.PolicyRequirements.DomainModelInterfaces;
using System;
using System.Runtime.Serialization;
namespace DALI.PolicyRequirements.DomainModels
{
    [KnownType(typeof(PolicyRequirementLevelModel))]
    public class PolicyRequirementLevelPropertyModel : IPolicyRequirementLevelPropertyModel, IPolicyRequirementLevelPropertyExportModel
    {
        public int Sequence { get; set; }

        public int Id { get; set; }

        public bool Active { get; set; }

        public int VersionId { get; set; }

        public string Description { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }
        public int LevelId { get; set; }
    }
}
