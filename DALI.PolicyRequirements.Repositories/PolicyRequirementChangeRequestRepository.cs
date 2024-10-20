using DALI.Enums;
using DALI.PolicyRequirements.DataEntityModels;
using DALI.PolicyRequirements.DomainModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;


namespace DALI.PolicyRequirements.Repositories
{
    public class PolicyRequirementChangeRequestRepository : Repository
    {
        public PolicyRequirementChangeRequestRepository(IPolicyRequirementDataEntityModelContext context) : base(context)
        {
        }

        private PolicyRequirementChangeRequest MapToEntity(PolicyRequirementChangeRequestModel model)
        {
            var entity = new PolicyRequirementChangeRequest();

            entity.Id = model.Id;
            entity.PolicyRequirementId = model.PolicyRequirementId;
            entity.VersionId = model.VersionId;
            entity.ActionId = model.ActionId;
            entity.IsApproved = model.IsApproved ? 1 : 0;
            entity.Owner = model.Owner;
            entity.UserName = model.UserName;
            entity.CreatedBy = model.CreatedBy;
            entity.CreatedDate = model.CreatedDate;
            entity.ModifiedBy = model.ModifiedBy;
            entity.ModifiedDate = model.ModifiedDate;
            entity.Remark = model.Remark;
            entity.Status = model.StatusId;

            return entity;
        }

        public async Task<bool> NotExists(int policyRequirementId, int versionId)
        {
            var entity = await _Context.PolicyRequirementChangeRequests.FirstOrDefaultAsync(e => e.PolicyRequirementId == policyRequirementId && e.VersionId == versionId);

            return entity == null;
        }

        private PolicyRequirementChangeRequestModel MapToModel(PolicyRequirementChangeRequest entity)
        {
            var model = new PolicyRequirementChangeRequestModel();

            model.Id = entity.Id;
            model.PolicyRequirementId = entity.PolicyRequirementId;
            model.StatusId = entity.Status;
            model.UserName = entity.UserName;
            model.Remark = entity.Remark;
            model.ShortRemark = (entity.Remark.Length > 50) ? string.Format("{0}...", entity.Remark.Substring(0, 50)) : entity.Remark;
            model.ActionId = entity.ActionId;
            model.CreatedDate = entity.CreatedDate;
            model.ModifiedDate = (!entity.ModifiedDate.HasValue) ? entity.CreatedDate : entity.ModifiedDate.Value;
            model.StatusDescription = ChangeRequestStatusEnum.Instance.List[entity.PolicyRequirementChangeRequestStatu.Id];
            model.ActionDescription = ReasonEnum.Instance.List[entity.PolicyRequirementChangeRequestAction.Id];
            model.Owner = entity.Owner;
            model.IsApproved = (!entity.IsApproved.HasValue) ? default(bool) : (entity.IsApproved.Value == ChangeRequestDecisionEnum.Approved);
            model.Tooltip = ChangeRequestStatusEnum.Instance.Tooltip[entity.Status];
            model.VersionId = entity.VersionId;

            return model;
        }

        public async Task<PolicyRequirementChangeRequestModel> New(bool withNewId)
        {
            var model = new PolicyRequirementChangeRequestModel();

            if (withNewId)
            {
                int nextId = await GetNextIdForEntity(new PolicyRequirementChangeRequest());

                model.Id = nextId;
            }

            return model;
        }

        public async Task<List<PolicyRequirementChangeRequestModel>> GetAll(int versionId, string owner)
        {
            var entities = await _Context.PolicyRequirementChangeRequests.Where(e => e.VersionId == versionId && e.Owner == owner).ToArrayAsync();

            var models = entities.Select(e => MapToModel(e)).ToList();

            return models;
        }

        public async Task<List<PolicyRequirementChangeRequestModel>> GetAll(int versionId, int policyRequirementId, string owner)
        {
            var entities = await _Context.PolicyRequirementChangeRequests.Where(e => e.VersionId == versionId && e.PolicyRequirementId == policyRequirementId && e.Owner == owner).ToArrayAsync();

            var models = entities.Select(e => MapToModel(e)).ToList();

            return models;
        }

        public async Task<List<PolicyRequirementChangeRequestModel>> GetAll(int versionId)
        {
            var entities = await _Context.PolicyRequirementChangeRequests.Where(e => e.VersionId == versionId).ToArrayAsync();

            var models = entities.Select(e => MapToModel(e)).ToList();

            return models;
        }

        public async Task<bool> IsRegistered(int policyRequirementId, int versionId)
        {
            var entity = await _Context.PolicyRequirementChangeRequests.SingleOrDefaultAsync(e => e.PolicyRequirementId == policyRequirementId && e.VersionId == versionId);

            return (entity != null && entity.Id > 0);
        }

        private async Task<PolicyRequirementChangeRequest> GetOneEntityAsync(int id, int versionId)
        {
            var entity = await _Context.PolicyRequirementChangeRequests.SingleOrDefaultAsync(e => e.Id == id && e.VersionId == versionId);

            return entity;
        }

        public async Task<PolicyRequirementChangeRequestModel> GetOneModelAsync(int id, int versionId)
        {
            var entity = await GetOneEntityAsync(id, versionId);

            if (entity != null)
            {
                var model = MapToModel(entity);
                return model;
            }

            return default(PolicyRequirementChangeRequestModel);
        }

        public async Task<int> Add(PolicyRequirementChangeRequestModel model)
        {
            int result = 0;

            var entity = new PolicyRequirementChangeRequest()
            {
                ActionId = model.ActionId,
                PolicyRequirementId = model.PolicyRequirementId,
                Remark = model.Remark,
                Status = model.StatusId,
                UserName = model.UserName,
                Owner = model.Owner,
                CreatedBy = model.CreatedBy,
                CreatedDate = model.CreatedDate,
                ModifiedBy = model.ModifiedBy,
                ModifiedDate = model.ModifiedDate,
                VersionId = model.VersionId,
            };

            if (model.IsApproved)
            {
                entity.IsApproved = ChangeRequestDecisionEnum.Approved;
            }

            int nextId = await GetNextIdForEntity(entity);

            entity.Id = nextId;

            _Context.PolicyRequirementChangeRequests.AddObject(entity);

            model.Id = nextId;

            result = 1;

            return result;
        }

        public async Task<int> Update(PolicyRequirementChangeRequestModel model)
        {
            var result = 0;

            var entity = await GetOneEntityAsync(model.Id, model.VersionId);

            if (null != entity)
            {
                if (model.IsApproved)
                {
                    entity.IsApproved = ChangeRequestDecisionEnum.Approved;
                }

                if (!model.IsApproved)
                {
                    entity.RejectMessage = model.RejectMessage;
                    entity.IsApproved = ChangeRequestDecisionEnum.Rejected;
                }

                if (!string.IsNullOrEmpty(model.Remark))
                    entity.Remark = model.Remark;

                entity.Status = model.StatusId;

                entity.ModifiedDate = DateTime.Now;

                result = 1;
            }

            return result;
        }

        public async Task<int> AssignNewOwner(int[] preqIds, string oldOwner, string newOwner, int versionId)
        {
            var result = 0;

            var entities = await _Context.PolicyRequirementChangeRequests.Where(e => string.Compare(e.Owner, oldOwner, true) == 0 && e.VersionId == versionId && preqIds.Contains(e.PolicyRequirementId)).ToArrayAsync();

            foreach (var entity in entities)
            {
                if (string.Compare(entity.Owner, newOwner, true) != 0)
                {
                    entity.Owner = newOwner;
                    if (result == 0)
                        result = 1;
                }
            }

            return result;
        }

        public async Task<int> Delete(int policyRequirementId, string createdBy, int versionId)
        {
            var result = 0;

            var entities = await _Context.PolicyRequirementChangeRequests.Where(e => e.VersionId == versionId && e.PolicyRequirementId == policyRequirementId && e.CreatedBy == createdBy).ToArrayAsync();

            foreach (var entity in entities)
            {
                _Context.PolicyRequirementChangeRequests.DeleteObject(entity);
            }

            result = 1;

            return result;
        }
    }
}
