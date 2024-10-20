using DALI.Enums;
using DALI.Models;
using DALI.PolicyRequirements.BusinessRules.Validators;
using DALI.PolicyRequirements.DomainModels;
using DALI.PolicyRequirements.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.PublicSpaceManagement.Services
{
    public partial class PublicSpaceManagementServices
    {
        #region ChangeRequests
        public async Task<List<PolicyRequirementChangeRequestModel>> GetChangeRequests(int version, string owner)
        {
            List<PolicyRequirementChangeRequestModel> models = await _ChangeRequestRepos.GetAll(version, owner);

            return models;
        }

        public async Task<List<PolicyRequirementChangeRequestModel>> GetChangeRequests(int version, int policyRequirementId, string owner)
        {
            List<PolicyRequirementChangeRequestModel> models = await _ChangeRequestRepos.GetAll(version, policyRequirementId, owner);

            return models;
        }

        public async Task<PolicyRequirementChangeRequestModel> GetChangeRequest(int id, int version)
        {
            PolicyRequirementChangeRequestModel model = await _ChangeRequestRepos.GetOneModelAsync(id, version);

            return model;
        }

        public async Task<ResponseModel> AddChangeRequest(PolicyRequirementModificationModel model, int action, int version)
        {
            var response = new ResponseModel();

            try
            {
                bool addChReqForOwner = await _ChangeRequestRepos.NotExists(model.Id, version);

                if (addChReqForOwner)
                {
                    PolicyRequirementChangeRequestModel crModel = new PolicyRequirementChangeRequestModel();

                    crModel.ActionId = action;
                    crModel.StatusId = ChangeRequestStatusEnum.InProgress;
                    crModel.UserName = model.ModifiedBy ?? model.CreatedBy;
                    crModel.VersionId = model.VersionId;
                    crModel.CreatedBy = model.ModifiedBy ?? model.CreatedBy;
                    crModel.CreatedDate = DateTime.Now;
                    crModel.ModifiedBy = model.ModifiedBy ?? model.CreatedBy;
                    crModel.ModifiedDate = crModel.CreatedDate;
                    crModel.Owner = model.Owner;
                    crModel.IsApproved = true;
                    crModel.PolicyRequirementId = model.Id;
                    crModel.Remark = Localization.OwnChangesTitle;

                    var crStatus = await _ChangeRequestRepos.Add(crModel);

                    if (crStatus == 1)
                    {
                        crStatus = await _ChangeRequestRepos.Save();
                    }


                    if (crStatus == 1)
                    {
                        response.Status = 1;
                        response.Id = model.Id;
                        response.Errors = null;
                    }
                    else
                    {
                        response.Status = 0;
                        response.Errors.Add("AddChangeRequest", "Cannot create a change request for the modification");
                    }
                }
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("AddChangeRequest", error.Message);
            }

            return response;
        }


        public async Task<ResponseModel> AddChangeRequest(PolicyRequirementChangeRequestModel model)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = model.CreatedDate;
                model.ModifiedBy = model.CreatedBy;
                model.UserName = model.CreatedBy;
                model.StatusId = ChangeRequestStatusEnum.New;

                Dictionary<string, string> validateResults = await ChangeRequestModelValidator.IsValidModel(model);

                if (validateResults.Count() == 0)
                {
                    var result = await _ChangeRequestRepos.Add(model);

                    if (result == 1)
                    {
                        result = await _ChangeRequestRepos.Save();
                    }

                    if (result == 1)
                    {
                        PolicyRequirementModificationModel prmModel = new PolicyRequirementModificationModel
                        {
                            Id = model.PolicyRequirementId,
                            VersionId = model.VersionId
                        };

                        response = await QueueModifications(prmModel);

                        if (response.Status == 0)
                            return response;
                    }

                    response.Status = 1;
                    response.Id = model.Id;
                    response.Errors = null;
                }
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("AddChangeRequest", error.Message);
            }

            return response;
        }

        public async Task<PolicyRequirementChangeRequestModel> New(bool withNewId)
        {
            PolicyRequirementChangeRequestModel result = await _ChangeRequestRepos.New(withNewId);

            return result;
        }

        public async Task<ResponseModel> AcceptChangeRequest(PolicyRequirementChangeRequestModel model)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                int status = await _ChangeRequestRepos.Update(model);

                if (status == 1)
                {
                    status = await _ChangeRequestRepos.Save();
                }

                if (status == 1)
                {
                    response.Status = 1;
                    response.Id = model.Id;
                    response.Errors = null;
                }
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("AcceptChangeRequest", error.Message);
            }

            return response;
        }

        public async Task<ResponseModel> RejectChangeRequest(PolicyRequirementChangeRequestModel model)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                int status = await _ChangeRequestRepos.Update(model);

                if (status == 1)
                {
                    status = await _ChangeRequestRepos.Save();
                }

                if (status == 1)
                {
                    response.Status = 1;
                    response.Id = model.Id;
                    response.Errors = null;
                }
            }
            catch (Exception error)
            {
                response.Status = 0;
                response.Errors.Add("RejectChangeRequest", error.Message);
            }

            return response;
        }

        public async Task<ResponseModel> DeleteChangeRequest(int policyRequirementId, int version, string createdBy)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                int result = await _ChangeRequestRepos.Delete(policyRequirementId, createdBy, version);

                if (result == 1)
                {
                    result = await _ChangeRequestRepos.Save();
                }

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