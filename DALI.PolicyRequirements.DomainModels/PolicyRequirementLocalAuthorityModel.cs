using DALI.PolicyRequirements.DomainModelInterfaces;
using DALI.Topics.SharedInfrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.DomainModels
{
    public class PolicyRequirementLocalAuthorityModel : IPolicyRequirementLocalAuthorityModel, IPolicyRequirementLocalAuthorityExportModel, ILocalAuthorityTopicModel
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
    }
}
