using DALI.PolicyRequirements.DomainModelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.DomainModels
{
    public class PolicyRequirementLevelExportModel : IPolicyRequirementLevelExportModel
    {
        public string Description { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Position { get; set; }
    }
}
