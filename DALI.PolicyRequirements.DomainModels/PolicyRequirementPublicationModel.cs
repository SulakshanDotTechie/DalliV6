using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace DALI.PolicyRequirements.DomainModels
{
    [KnownType(typeof(PolicyRequirementPublicationModel))]
    public class PolicyRequirementPublicationModel : IPolicyRequirementPublicationModel
    {
        public int Id { get; set; }

        public int VersionId { get; set; }

        public string Description { get; set; }
        public string PublishedBy { get; set; }

        public bool? IsPublished { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        [IgnoreDataMember]
        public string Info { get; set; }
    }
}
