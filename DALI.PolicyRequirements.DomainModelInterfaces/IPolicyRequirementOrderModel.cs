using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DALI.PolicyRequirements.DomainModels
{
    public interface IPolicyRequirementOrderModel
    {
        int PolicyRequirementId { get; set; }
        int Index { get; set; }
    }
}