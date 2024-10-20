using System.ComponentModel.DataAnnotations;
using System;
using System.Runtime.Serialization;
using System.Net;
using System.Collections.Generic;

namespace DALI.PolicyRequirements.DomainModels
{
    [KnownType(typeof(PolicyRequirementPublicationQueueModel))]
    public class PolicyRequirementPublicationQueueModel : IPolicyRequirementPublicationQueueModel
    {
        public int Id { get; set; }

        public bool Active { get; set; }

        public int VersionId { get; set; }

        public bool? CanPublish { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string Comment { get; set; }
    }
}
