using DALI.Models;
using DALI.PolicyRequirements.BusinessRules.Validators;
using DALI.PolicyRequirements.DomainModels;
using DALI.PublicSpaceManagement.Shared;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.PublicSpaceManagement.Services
{
    public partial class PublicSpaceManagementServices
    {
        #region Attachments
        public async Task<PolicyRequirementAttachmentModel> GetAttachment(int id)
        {
            PolicyRequirementAttachmentModel model = await _AttachmentRepos.GetOneModelAsync(id);

            return model;
        }

        public async Task<PolicyRequirementAttachmentModel> GetAttachment(string fileName)
        {
            PolicyRequirementAttachmentModel model = await _AttachmentRepos.GetOneModelAsync(fileName);

            return model;
        }

        public async Task<List<PolicyRequirementAttachmentModel>> GetAttachments()
        {
            List<PolicyRequirementAttachmentModel> models = await _AttachmentRepos.GetAll();

            return models.Where(m => m.Active).OrderBy(e => e.Description).ToList();
        }

        /// <summary>
        /// Get assigned attachments by a specific version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public async Task<List<PolicyRequirementAttachmentModel>> GetAttachments(int version)
        {
            var models = await _AttachmentRepos.GetAssigned(version);

            return models.OrderBy(e => e.Description).ToList();
        }

        /// <summary>
        /// Get assigned attachments for a policy requirement and a specific version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public async Task<List<PolicyRequirementAttachmentModel>> GetAttachments(int policyRequirementId, int version)
        {
            var models = await _AttachmentRepos.GetAssigned(policyRequirementId, version);

            return models.OrderBy(e => e.IsAssigned).ThenBy(e => e.Description).ToList();
        }

        /// <summary>
        /// Get the attachments which are queued for a new publication for a specific version
        /// </summary>
        /// <param name="policyRequirementId"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public async Task<List<PolicyRequirementAttachmentModel>> GetQueuedAttachments(int policyRequirementId, int version)
        {
            List<PolicyRequirementAttachmentModel> models = await _AttachmentRepos.GetQueued(policyRequirementId, version);

            return models.OrderBy(e => e.IsAssigned).ThenBy(e => e.Description).ToList();
        }

        public async Task<ResponseModel> QueueAttachments(int policyRequirementId, int version, IList<int> models)
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
                var prmModel = await _ModificationQueueRepos.GetOneModelAsync(policyRequirementId, version);

                if (prmModel != null)
                {
                    foreach (int id in models)
                    {
                        int result = await _AttachmentRepos.AddToQueue(id, policyRequirementId, version);
                    }

                    response.Status = 1;

                    if (response.Status == 1)
                    {
                        int result = await _AttachmentRepos.Save();

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

        public async Task<ResponseModel> DequeueAttachments(int policyRequirementId, int version, IList<int> models)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                foreach (var id in models)
                {
                    int delStatus = await _AttachmentRepos.DeleteFromQueue(id, policyRequirementId, version);
                }

                int result = await _AttachmentRepos.Save();

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

        public async Task<ResponseModel> AddAttachment(PolicyRequirementMediaModel model, string storageLocation)
        {
            ResponseModel response = new ResponseModel();

            var fName = Path.GetFileName(model.FileName).Replace(" ", "_");
            var newModel = new PolicyRequirementAttachmentModel();
            newModel.Description = model.LabelName;
            newModel.StorageLocation = string.Format("{0}{1}", storageLocation, fName);
            newModel.FileName = fName;
            newModel.Active = true;

            // att document currently exists?
            PolicyRequirementAttachmentModel attModel = await GetAttachment(newModel.Id);

            // Add new att
            if (attModel == null)
            {
                Dictionary<string, string> validateResult = await PolicyRequirementAttachmentModelValidator.IsValidModel(newModel);
                bool isValidModel = validateResult.Count() == 0;

                if (isValidModel)
                {
                    int status = await _AttachmentRepos.Add(newModel);

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
                response.Errors.Add("ErrorMessage", string.Format("The attachment file {0} already exists", attModel.FileName));
            }

            return response;
        }
        #endregion
    }
}