using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DALI.PolicyRequirements.DomainModels
{
    [KnownType(typeof(PolicyRequirementChangeRequestModel))]
    public class PolicyRequirementChangeRequestModel : IPolicyRequirementChangeRequestModel
    {

        public int Id { get; set; }

        public int VersionId { get; set; }

        public string Description { get; set; }

        public bool Active { get; set; }

        public int PolicyRequirementId { get; set; }

        public int ActionId { get; set; }

        public int StatusId { get; set; }

        public bool IsApproved { get; set; }

        public string UserName { get; set; }

        public string Remark { get; set; }

        public string ShortRemark { get; set; }

        public string StatusDescription { get; set; }

        public string ActionDescription { get; set; }

        public string AdditionalInfo { get; set; }

        public string Owner { get; set; }

        public string Tooltip { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string RejectMessage { get; set; }
    }
}
