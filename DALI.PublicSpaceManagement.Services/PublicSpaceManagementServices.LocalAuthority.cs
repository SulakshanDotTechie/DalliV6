using DALI.PolicyRequirements.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.PublicSpaceManagement.Services
{
    public partial class PublicSpaceManagementServices
    {
        public async Task<List<PolicyRequirementLocalAuthorityModel>> GetLocalAuthorities()
        {
            List<PolicyRequirementLocalAuthorityModel> models = await _LocalAuthorityRepos.GetAll();

            return models;
        }

        public async Task<PolicyRequirementLocalAuthorityModel> GetLocalAuthority(Guid id)
        {
            PolicyRequirementLocalAuthorityModel model = await _LocalAuthorityRepos.GetOneModelAsync(id);

            return model;
        }
    }
}
