﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.DesignBrief.DataEntityModels
{
    public partial class DesignBriefPolicyRequirementStrictness : IDesignBriefPolicyRequirementStrictness
    {
        public string TableName
        {
            get
            {
                return "DesignBriefPolicyRequirementStrictness";
            }
        }
    }
}
