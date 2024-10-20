using DALI.Models;
using DALI.PolicyRequirements.BusinessRules.Validators;
using DALI.PolicyRequirements.DomainModels;
using DALI.PublicSpaceManagement.Shared;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DALI.PublicSpaceManagement.Services
{
    public partial class PublicSpaceManagementServices
    {
        #region SourceDocuments
        private async Task<bool> IsValidUrlAsync(string url)
        {
            try
            {
                var request = WebRequest.Create(url);
                request.Timeout = 5000;
                request.Method = "HEAD";

                var response = await request.GetResponseAsync();
                var result = (HttpWebResponse)response;

                result.Close();
                return result.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }

        private async Task SetIsOnlineFlag(List<PolicyRequirementSourceDocumentModel> models)
        {
            List<Task<PolicyRequirementSourceDocumentModel>> tasks = new List<Task<PolicyRequirementSourceDocumentModel>>();

            foreach (var model in models)
            {
                tasks.Add(IsValidUrlTask(model));
            }

            var results = await Task.WhenAll(tasks);
        }

        private async Task<PolicyRequirementSourceDocumentModel> IsValidUrlTask(PolicyRequirementSourceDocumentModel model)
        {
            model.IsOnline = string.IsNullOrEmpty(model.StorageLocation) ? false : await IsValidUrlAsync(model.StorageLocation);

            return model;
        }

        public async Task<PolicyRequirementSourceDocumentModel> GetSourceDocument(int id)
        {
            PolicyRequirementSourceDocumentModel model = await _SourceReferenceRepos.GetOneModelAsync(id);

            return model;
        }

        public async Task<PolicyRequirementSourceDocumentModel> GetSourceDocument(string description)
        {
            PolicyRequirementSourceDocumentModel model = await _SourceReferenceRepos.GetOneModelAsync(description);

            return model;
        }

        public async Task<List<PolicyRequirementSourceDocumentModel>> GetSourceDocuments()
        {
            List<PolicyRequirementSourceDocumentModel> models = await _SourceReferenceRepos.GetAll();

            //await SetIsOnlineFlag(models);

            return models.Where(m => m.Active).OrderBy(m => m.Description).ToList();
        }

        public async Task<List<PolicyRequirementSourceDocumentModel>> GetSourceDocuments(int version)
        {
            List<PolicyRequirementSourceDocumentModel> models = await _SourceReferenceRepos.GetAssigned(version);

            //await SetIsOnlineFlag(models);

            return models.OrderBy(e => e.IsAssigned).ThenBy(m => m.Description).ToList();
        }

        public async Task<List<PolicyRequirementSourceDocumentModel>> GetSourceDocuments(int policyRequirementId, int version)
        {
            List<PolicyRequirementSourceDocumentModel> models = await _SourceReferenceRepos.GetAssigned(policyRequirementId, version);

            //await SetIsOnlineFlag(models);

            return models.OrderBy(e => e.IsAssigned).ThenBy(m => m.Description).ToList();
        }

        public async Task<List<PolicyRequirementSourceDocumentModel>> GetQueuedSourceDocuments(int policyRequirementId, int version)
        {
            List<PolicyRequirementSourceDocumentModel> models = await _SourceReferenceRepos.GetQueued(policyRequirementId, version);

            //await SetIsOnlineFlag(models);

            return models.OrderBy(e => e.IsAssigned).ThenBy(m => m.Description).ToList();
        }

        public async Task<ResponseModel> QueueSourceDocuments(int policyRequirementId, int version, IList<int> models)
        {
            var response = new ResponseModel
            {
                Status = 0
            };

            if (models.Count == 0)
            {
                response.Status = 1;
                response.Errors.Add("QueueSourceDocuments", "Currently there are no sourcedocuments available");
            }
            else
            {
                // the model for modification must be queued first before attachments can be queued
                PolicyRequirementModificationModel prmModel = await _ModificationQueueRepos.GetOneModelAsync(policyRequirementId, version);

                if (prmModel != null)
                {
                    foreach (var id in models)
                    {
                        int result = await _SourceReferenceRepos.AddToQueue(id, policyRequirementId, version);
                    }

                    response.Status = 1;

                    if (response.Status == 1)
                    {
                        var result = await _SourceReferenceRepos.Save();

                        response.Status = 1;
                        response.Errors = null;
                    }
                }
                else
                {
                    response.Status = 0;
                    response.Errors.Add("QueueSourceDocuments", "Queued PolicyRequirement model not found");
                }
            }

            return response;
        }

        public async Task<ResponseModel> DequeueSourceDocuments(int policyRequirementId, int version, IList<int> models)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                foreach (var id in models)
                {
                    int delStatus = await _SourceReferenceRepos.DeleteFromQueue(id, policyRequirementId, version);
                }

                int result = await _SourceReferenceRepos.Save();

                response.Status = 1;
                response.Errors = null;
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("DequeueSourceDocuments", error.Message);
            }

            return response;
        }

        public async Task<ResponseModel> AddSourceDocument(PolicyRequirementMediaModel model)
        {
            ResponseModel response = new ResponseModel();

            var newModel = new PolicyRequirementSourceDocumentModel();
            newModel.Description = model.LabelName;
            newModel.StorageLocation = model.Url;
            newModel.Active = true;

            // source document currently exists?
            PolicyRequirementSourceDocumentModel srcModel = await GetSourceDocument(newModel.Id);

            // Add new source document
            if (srcModel == null)
            {
                Dictionary<string, string> validateResult = await PolicyRequirementSourceReferenceModelValidator.IsValidModel(newModel);
                bool isValidModel = validateResult.Count() == 0;

                if (isValidModel)
                {
                    int status = await _SourceReferenceRepos.Add(newModel);

                    if (status == 1)
                    {
                        response.Status = 1;
                        response.Id = newModel.Id;
                        response.Errors = null;
                    }
                    else
                        throw new InvalidOperationException();
                }
                else
                {
                    foreach (var error in validateResult)
                    {
                        response.Status = 0;
                        response.Errors.Add(error.Key, error.Value);
                    }
                }
            }
            else
            {
                response.Status = 0;
                response.Errors.Add("ErrorMessage", string.Format("The source reference {0} already exists", srcModel.Description));
            }

            return response;
        }
        #endregion
    }
}