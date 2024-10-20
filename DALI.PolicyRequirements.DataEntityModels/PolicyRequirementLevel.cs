using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DALI.PolicyRequirements.DataEntityModels
{
    public partial class PolicyRequirementLevel : IPolicyRequirementLevelEntity
    {
        public string TableName
        {
            get
            {
                return "PolicyRequirementLevel";
            }
        }
    }
}
