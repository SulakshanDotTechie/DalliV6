using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DALI.PolicyRequirements.DomainModels
{
    [KnownType(typeof(PolicyRequirementOrderModel))]
    public class PolicyRequirementOrderModel : IPolicyRequirementOrderModel
    {
        public int PolicyRequirementId { get; set; }
        public int Index { get; set; }
    }
}