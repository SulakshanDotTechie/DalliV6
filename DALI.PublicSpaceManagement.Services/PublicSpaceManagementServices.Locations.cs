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
        public async Task<List<PolicyRequirementLocationModel>> GetLocations()
        {
            List<PolicyRequirementLocationModel> models = await _LocationRepos.GetAll();

            return models;
            //.Where(e => e.FetchByDefault == false).ToList();
        }

        public async Task<List<PolicyRequirementLocationModel>> GetLocationsForEditors()
        {
            List<PolicyRequirementLocationModel> models = await _LocationRepos.GetAll();

            return models;
        }

        public async Task<PolicyRequirementLocationModel> GetLocation(Guid id)
        {
            PolicyRequirementLocationModel model = await _LocationRepos.GetOneModelAsync(id);

            return model;
        }
    }
}
