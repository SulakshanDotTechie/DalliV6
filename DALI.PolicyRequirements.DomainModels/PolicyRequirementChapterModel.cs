using System.ComponentModel.DataAnnotations;
using System;
using System.Runtime.Serialization;
using DALI.PolicyRequirements.DomainModelInterfaces;
using DALI.Topics.SharedInfrastructure;

namespace DALI.PolicyRequirements.DomainModels
{
    [KnownType(typeof(PolicyRequirementChapterModel))]
    public class PolicyRequirementChapterModel : IPolicyRequirementChapterModel, IPolicyRequirementChapterExportModel, IChapterTopicModel
    {
        public int Id { get; set; }
        public string ChapterNumber { get; set; }
        public string Owner { get; set; }
        public string OwnerEmailAddress { get; set; }

        public string FullChapterDescription
        {
            get
            {
                return string.Format("{0}. {1}", ChapterNumber, Description);
            }
        }

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
