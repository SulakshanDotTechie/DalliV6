using DALI.ExportEngine.Models;
using DALI.Models;
using DALI.PolicyRequirements.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.PublicSpaceManagement.Services
{
    public partial class PublicSpaceManagementServices
    {
        #region Strictness
        public async Task<List<PolicyRequirementSeverityModel>> GetSeverities()
        {
            List<PolicyRequirementSeverityModel> models = await _SeverityRepos.GetAll();

            return models.Where(m => m.Active).ToList();
        }

        public async Task<List<PolicyRequirementSeverityModel>> GetSeverities(int version)
        {
            List<PolicyRequirementSeverityModel> models = await _SeverityRepos.GetAssigned(version);

            return models;
        }

        public async Task<List<PolicyRequirementSeverityModel>> GetSeverities(int policyRequirementId, int version)
        {
            List<PolicyRequirementSeverityModel> models = await _SeverityRepos.GetAssigned(policyRequirementId, version);

            return models;
        }

        public async Task<List<PolicyRequirementSeverityModel>> GetQueuedSeverities(int policyRequirementId, int version)
        {
            List<PolicyRequirementSeverityModel> models = await _SeverityRepos.GetQueued(policyRequirementId, version);

            return models;
        }

        public async Task<ResponseModel> QueueSeverities(int policyRequirementId, int version, IList<int> models)
        {
            var response = new ResponseModel
            {
                Status = 0
            };

            if (models.Count == 0)
            {
                response.Status = 1;
                response.Errors.Add("ErrorMessage", "Currently there are no attachments available");
            }
            else
            {
                // the model for modification must be queued first before attachments can be queued
                PolicyRequirementModificationModel prmModel = await _ModificationQueueRepos.GetOneModelAsync(policyRequirementId, version);

                if (prmModel != null)
                {
                    foreach (var id in models)
                    {
                        int result = await _SeverityRepos.AddToQueue(id, policyRequirementId, version);
                    }

                    response.Status = 1;

                    if (response.Status == 1)
                    {
                        int result = await _SeverityRepos.Save();

                        response.Status = 1;
                    }
                }
                else
                {
                    response.Status = 0;
                    response.Errors.Add("ErrorMessage", "Queued PolicyRequirement model not found");
                }
            }

            return response;
        }

        public async Task<ResponseModel> DequeueSeverities(int policyRequirementId, int version, IList<int> models)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                foreach (var id in models)
                {
                    int delStatus = await _SeverityRepos.DeleteFromQueue(id, policyRequirementId, version);
                }

                int result = await _SeverityRepos.Save();

                response.Status = 1;
                response.Errors = null;
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("ErrorMessage", error.Message);
            }

            return response;
        }
        #endregion
    }
}