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
        public async Task<List<PolicyRequirementChildSubjectModel>> GetChildSubjectsForEditors(int version)
        {
            List<PolicyRequirementChildSubjectModel> result = await _ChildSubjectRepos.GetForEditors(version);

            return result;
        }

        public async Task<List<PolicyRequirementChildSubjectModel>> GetChildSubjects(int version)
        {
            List<PolicyRequirementChildSubjectModel> result = await _ChildSubjectRepos.GetAll(version);

            return result;
        }

        public async Task<PolicyRequirementChildSubjectModel> GetChildSubject(int id, int version)
        {
            PolicyRequirementChildSubjectModel result = await _ChildSubjectRepos.GetOneModelAsync(id, version);

            return result;
        }

        public async Task<List<PolicyRequirementChildSubjectModel>> GetChildSubjects(int[] chapters, int[] levels, Guid[] locations, int[] areas, int[] subjects, int version, List<PolicyRequirementModel> policyRequirements)
        {
            List<PolicyRequirementChildSubjectModel> models = await _ChildSubjectRepos.GetAll(chapters, levels, locations, areas, subjects, version, policyRequirements);

            return models;
        }
    }
}
