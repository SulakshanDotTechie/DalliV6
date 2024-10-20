using DALI.Models;
using DALI.PolicyRequirements.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DALI.SearchEngine.Models;


namespace DALI.PublicSpaceManagement.Services
{
    public partial class PublicSpaceManagementServices
    {
        #region Search
        public async Task<List<PolicyRequirementModel>> Search(LiorParametersModel parameters, List<PolicyRequirementModel> models)
        {
            // perform a first complex filtering based on pathnames
            var results = SearchEngine.SearchEngine.Get(parameters, models);

            if (!string.IsNullOrEmpty(parameters.Keyword))
            {
                // search for models by attachments
                int[] idsByAttachments = await _PolReqsRepos.GetByAttachments(parameters.VersionId, parameters.Keyword);

                if (idsByAttachments != null && idsByAttachments.Length > 0)
                {
                    // find the models in de specified list of models
                    IEnumerable<PolicyRequirementModel> filteredModelsWithAttachments = models.Where(m => idsByAttachments.Contains(m.Id)).Distinct();

                    filteredModelsWithAttachments = filteredModelsWithAttachments.Select(m => { m.HasAttachments = 2; return m; });

                    // only get the models which do not appear in the results
                    var modelsWithAttachments = filteredModelsWithAttachments.Where(m => !results.Any(b => b.Id == m.Id));

                    // add models which not appeared in the current results
                    results.AddRange(modelsWithAttachments);
                }

                int[] idsBySourReferences = await _PolReqsRepos.GetBySourceReferences(parameters.VersionId, parameters.Keyword);

                if (idsBySourReferences != null && idsBySourReferences.Length > 0)
                {
                    // find the models in de specified list of models
                    IEnumerable<PolicyRequirementModel> filteredModelsWithSourceRefs = models.Where(m => idsBySourReferences.Contains(m.Id)).Distinct();

                    filteredModelsWithSourceRefs = filteredModelsWithSourceRefs.Select(m => { m.HasSourceReferences = 2; return m; });

                    // only get the models which do not appear in the results
                    IEnumerable<PolicyRequirementModel> modelsWithSourceRefs = filteredModelsWithSourceRefs.Where(m => !results.Any(b => b.Id == m.Id));

                    // add models which not appeared in the current results
                    results.AddRange(modelsWithSourceRefs);
                }

                // reset the flag hasAttachments when no matches are found
                if (idsByAttachments == null || idsByAttachments.Length == 0)
                {
                    results = results.Select(m => { if (m.HasAttachments > 0) m.HasAttachments = 1; return m; }).ToList();
                }

                // reset the flag hasSourceReferences when no matches are found
                if (idsBySourReferences == null || idsBySourReferences.Length == 0)
                {
                    results = results.Select(m => { if (m.HasSourceReferences > 0) m.HasSourceReferences = 1; return m; }).ToList();
                }
            }

            return results;
        }
        #endregion
    }
}