using DALI.Models;
using DALI.PolicyRequirements.DomainModels;
using DALI.PublicSpaceManagement.SchedulerServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace DALI.PublicSpaceManagement.Services
{
    public partial class PublicSpaceManagementServices
    {
        #region Publications
        public async Task<List<PolicyRequirementPublicationModel>> GetPublications()
        {
            List<PolicyRequirementPublicationModel> models = await _PublicationRepos.GetAll();

            return models;
        }

        public async Task<PolicyRequirementPublicationModel> GetScheduledPublication()
        {
            PolicyRequirementPublicationModel model = await _PublicationRepos.GetScheduled();

            return model;
        }

        public async Task<ResponseModel> SchedulePublication(PolicyRequirementPublicationModel model)
        {
            ResponseModel response = new ResponseModel();

            try
            {

                PolicyRequirementPublicationModel scheduled = await _PublicationRepos.GetScheduled();

                if (scheduled != null)
                {
                    response.Status = 0;
                    response.Errors.Add("SchedulePublication", DALI.PolicyRequirements.Resources.Localization.ErrorUnpublishedVersionScheduled);
                }
                else
                {
                    int status = await _PublicationRepos.Add(model);

                    if (status == 1)
                    {
                        status = await _PublicationRepos.Save();

                        if (status == 1)
                        {
                            string taskName = WebConfigurationManager.AppSettings["TaskSchedulerTaskName"];
                            string taskDescription = string.Format("{0}, {1}: {2}", taskName, DALI.PolicyRequirements.Resources.Localization.Version, model.Id);

                            TaskSchedulerServiceHelper.Add(model.Id, model.StartTime, model.CreatedBy, taskDescription);
                        }
                    }

                    response.Status = 1;
                    response.Errors = null;
                }
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("SchedulePublication", error.Message);
            }

            return response;
        }

        public async Task<ResponseModel> DeleteScheduledPublication(int id)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                PolicyRequirementPublicationModel model = await _PublicationRepos.GetOneModelAsync(id);

                if (null != model)
                {
                    string taskName = WebConfigurationManager.AppSettings["TaskSchedulerTaskName"];

                    TaskSchedulerServiceHelper.RemoveScheduledTask(taskName, model.StartTime);

                    int status = await _PublicationRepos.Delete(model.Id);

                    if (status == 1)
                    {
                        status = await _PublicationRepos.Save();
                    }

                    response.Status = 1;
                    response.Errors = null;
                }
                else
                {
                    throw new Exception("Publication info cannot be found!");
                }
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("DeletePublication", error.Message);
            }

            return response;
        }

        public async Task<List<PolicyRequirementPublicationQueueModel>> GetQueuedPublications()
        {
            List<PolicyRequirementPublicationQueueModel> models = await _PublicationQueueRepos.GetAll();

            return models;
        }

        public async Task<PolicyRequirementPublicationQueueModel> GetQueuedPublication(int policyRequirementId)
        {
            PolicyRequirementPublicationQueueModel model = await _PublicationQueueRepos.GetOneModelAsync(policyRequirementId);

            return model;
        }

        public async Task<ResponseModel> QueuePublication(PolicyRequirementPublicationQueueModel model, int versionId)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var queuedModel = await _ModificationQueueRepos.GetOneModelAsync(model.Id, versionId);

                // Queue the modified model for publication
                if (queuedModel != null)
                {
                    var pqModel = new PolicyRequirementPublicationQueueModel
                    {
                        Id = model.Id,
                        Active = model.Active,
                        CreatedDate = DateTime.Now
                    };

                    int result = await _PublicationQueueRepos.Add(pqModel);

                    if (result == 1)
                    {
                        result = await _PublicationQueueRepos.Save();
                    }

                    response.Status = 1;
                    response.Id = pqModel.Id;
                    response.Errors = null;
                }
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("QueuePublication", error.Message);
            }

            return response;
        }

        public async Task<ResponseModel> UpdatePublication(PolicyRequirementPublicationQueueModel model)
        {
            var response = new ResponseModel();

            try
            {
                PolicyRequirementPublicationQueueModel queuedModel = await _PublicationQueueRepos.GetOneModelAsync(model.Id);

                // Queue the modified model for publication
                if (queuedModel != null)
                {

                    queuedModel.CanPublish = model.CanPublish;

                    int result = await _PublicationQueueRepos.Update(queuedModel);

                    if (result == 1)
                    {
                        result = await _PublicationQueueRepos.Save();
                    }

                    response.Status = 1;
                    response.Id = queuedModel.Id;
                    response.Errors = null;
                }
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("QueuePublication", error.Message);
            }

            return response;
        }

        public async Task<ResponseModel> DequeuePublication(int id)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                int result = await _PublicationQueueRepos.Delete(id);

                if (result == 1)
                {
                    result = await _PublicationQueueRepos.Save();
                }


                response.Status = 1;
                response.Errors = null;
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("DequeuePublication", error.Message);
            }

            return response;
        }

        public async Task<string[]> GetComments(int policyRequirementId)
        {
            string[] models = await _CommentRepos.GetAll(policyRequirementId);

            return models;
        }

        public async Task<string[]> GetComments(int policyRequirementId, string userName)
        {
            string[] models = await _CommentRepos.GetAll(policyRequirementId, userName);

            return models;
        }

        public async Task<ResponseModel> AddComment(int policyRequirementId, string comment, string userName, int version, int? action)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                FluentValidation.Results.ValidationResult valRes = _CommentValidator.Validate(new PolicyRequirementAdditionalInfoModel { Description = comment });

                if (valRes.IsValid)
                {
                    int result = await _CommentRepos.Add(comment, policyRequirementId, string.Empty, userName, action, version);
                    if (result == 1)
                    {
                        result = await _CommentRepos.Save();
                    }

                    response.Status = 1;
                    response.Errors = null;
                }
                else
                    throw new Exception(valRes.Errors[0].ErrorMessage);
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("AddComment", error.Message);
            }

            return response;
        }

        public async Task<int> GetDefaultPublicationScheduleStartTime()
        {
            return await _PublicationRepos.GetDefaultPublicationScheduleStartTime();
        }
        #endregion
    }
}