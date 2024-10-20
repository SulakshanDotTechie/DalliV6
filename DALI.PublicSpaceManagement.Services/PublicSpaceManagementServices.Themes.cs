using DALI.PublicSpaceManagement.DomainModels;
using DALI.SearchEngine.Models;
using DALI.Topics.Infrastructure.Helpers;
using DALI.Topics.Infrastructure.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.PublicSpaceManagement.Services
{
    public partial class PublicSpaceManagementServices
    {
        public async Task<ThemeTopicResponseModel> GetThemeReferences(LiorParametersModel parameters, int versionId)
        {
            ThemeTopicResponseModel responseModel = new ThemeTopicResponseModel("");

            var models = await _PublicSpaceManagementThemeRepos.GetAll(parameters.Themes, versionId);

            foreach (var model in models)
            {
                var pmodels = await _PolReqsRepos.GetAll(model.PolicyRequirementIds.ToArray(), versionId);

                var filteredResults = Search(parameters, pmodels).Result;

                if (filteredResults.Count > 0)
                {
                    var results = new TopicTreeList(parameters.Keyword).Transform(filteredResults.ToList<ITopicDetailModel>());

                    ThemeTopic t = new ThemeTopic();

                    t.Model = model.Theme;
                    t.Name = model.Theme.Description;
                    t.Topics.AddRange(results.Results);
                    t.UniqueId = string.Format("theme-{0}", model.Theme.Id);

                    responseModel.Results.Add(t);

                    responseModel.TotalResults += results.TotalResults;
                }
            }

            return responseModel;
        }

        public async Task<PublicSpaceManagementThemeModel[]> GetThemes()
        {
            var models = await _PublicSpaceManagementThemeRepos.GetAll();

            return models;
        }

        public async Task<PublicSpaceManagementThemeModel[]> GetByTheme(int policyRequirementId, int versionId)
        {
            var models = await _PublicSpaceManagementThemeRepos.GetByPolicyRequirement(policyRequirementId, versionId);

            return models;
        }
    }
}
