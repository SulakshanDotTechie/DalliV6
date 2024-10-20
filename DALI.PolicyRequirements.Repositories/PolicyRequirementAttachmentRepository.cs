using DALI.Enums;
using DALI.PolicyRequirements.DataEntityModels;
using DALI.PolicyRequirements.DomainModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.Repositories
{
    public class PolicyRequirementAttachmentRepository : Repository
    {
        public PolicyRequirementAttachmentRepository(IPolicyRequirementDataEntityModelContext context) : base(context)
        {
        }

        private PolicyRequirementAttachment MapToEntity(PolicyRequirementAttachmentModel model)
        {
            var entity = new PolicyRequirementAttachment();

            entity.ID = model.Id;
            entity.Description = model.Description;
            entity.StorageLocation = model.StorageLocation;
            entity.IsActive = model.Active ? 1 : 0;
            entity.FileName = model.FileName;

            return entity;
        }

        private PolicyRequirementAttachmentModel MapToModel(PolicyRequirementAttachment entity)
        {
            var model = new PolicyRequirementAttachmentModel();

            // Fill model properties
            model.Id = entity.ID;
            model.Active = (entity.IsActive == 1);
            model.FileName = entity.FileName;
            model.Description = entity.Description;
            model.StorageLocation = (!string.IsNullOrEmpty(entity.StorageLocation) && entity.StorageLocation.Contains(_FileFolder)) ? entity.StorageLocation.Replace("~/", _BaseUrl) : entity.StorageLocation;
            model.BaseLocation = entity.StorageLocation;
            model.FileExtension = System.IO.Path.GetExtension(model.FileName);

            return model;
        }

        private Task<PolicyRequirementAttachment> GetOneEntityAsync(int id)
        {
            return Task.FromResult(_Context.PolicyRequirementAttachments.SingleOrDefault(e => e.ID == id));
        }

        public async Task<int> Add(PolicyRequirementAttachmentModel model)
        {
            int result = 0;

            var entity = MapToEntity(model);

            int nextId = await GetNextIdForEntity(entity);

            entity.ID = nextId;

            _Context.PolicyRequirementAttachments.AddObject(entity);

            await _Context.SaveChangesAsync();

            model.Id = nextId;

            result = 1;

            return result;
        }

        public int AddWithInitializedId(PolicyRequirementAttachmentModel model)
        {
            int result = 0;

            var entity = MapToEntity(model);

            _Context.PolicyRequirementAttachments.AddObject(entity);

            result = 1;

            return result;
        }

        public async Task<List<PolicyRequirementAttachmentModel>> GetAll()
        {
            var entities = await _Context.PolicyRequirementAttachments.ToArrayAsync();

            var models = entities.Select(e => MapToModel(e)).OrderBy(e => e.Description).ToList();

            return models;
        }

        public async Task<PolicyRequirementAttachmentModel[]> GetAssigned(int versionId)
        {
            var ids = await _Context.PolicyRequirementAssignedAttachments.Where(e => e.VersionId == versionId && e.IsActive == 1).Select(e => e.AttachmentId).Distinct().ToArrayAsync();

            var entities = await _Context.PolicyRequirementAttachments.Where(e => ids.Contains(e.ID)).ToArrayAsync();

            var models = entities.Select(e => ToModel(e, versionId)).OrderBy(e => e.Description).ToArray();

            return models;
        }


        public PolicyRequirementAttachmentModel ToModel(PolicyRequirementAttachment entity, int versionId)
        {
            var model = MapToModel(entity);

            model.IsAssigned = true;
            model.PolicyRequirementId = entity.ID;
            model.VersionId = versionId;

            return model;
        }

        public async Task<PolicyRequirementAttachmentModel[]> GetAssigned(int policyRequirementId, int versionId)
        {
            var entities = await _Context.PolicyRequirementAssignedAttachments.Where(e => e.PolicyRequirementId == policyRequirementId && e.VersionId == versionId && e.IsActive == 1).ToArrayAsync();

            var models = entities.Select(e => ToModel(e.PolicyRequirementAttachment, versionId)).ToArray();

            return models;
        }

        public async Task<PolicyRequirementAttachmentModel> GetOneModelAsync(int id)
        {
            var entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                var model = MapToModel(entity);

                return model;
            }

            return default(PolicyRequirementAttachmentModel);
        }

        public async Task<PolicyRequirementAttachmentModel> GetOneModelAsync(string fileName)
        {
            var entity = await _Context.PolicyRequirementAttachments.SingleOrDefaultAsync(e => string.Compare(e.FileName, fileName, true) == 0);

            if (entity != null)
            {
                var model = MapToModel(entity);

                return model;
            }

            return default(PolicyRequirementAttachmentModel);
        }

        public async Task<int> Update(PolicyRequirementAttachmentModel model, bool fullUpdate)
        {
            int result = 0;

            var entity = await GetOneEntityAsync(model.Id);

            if (entity != null)
            {
                entity.Description = model.Description;
                entity.IsActive = (model.Active ? 1 : 0);

                if (fullUpdate)
                {
                    entity.FileName = model.FileName;
                    entity.StorageLocation = model.StorageLocation;
                }

                result = 1;
            }

            return result;
        }

        public async Task<int> Update(PolicyRequirementAttachmentModel model)
        {
            return await Update(model, false);
        }

        public async Task<int> Delete(int id)
        {
            int result = 0;

            var entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                _Context.PolicyRequirementAttachments.DeleteObject(entity);

                result = 1;
            }

            return result;
        }

        public async Task<List<PolicyRequirementAttachmentModel>> GetQueued(int versionId)
        {
            int iAssignmentEnum = AssignmentEnum.Attachment.AsInt();

            var ids = await _Context.PolicyRequirementModificationAssignmentQueues.Where(e => e.VersionId == versionId && e.AssignmentTypeId == iAssignmentEnum).Select(e => e.Id).Distinct().ToArrayAsync();

            var entities = await _Context.PolicyRequirementAttachments.Where(e => ids.Contains(e.ID)).ToArrayAsync();

            var models = entities.Select(e => MapToModel(e)).ToList();

            return models;
        }

        public async Task<List<PolicyRequirementAttachmentModel>> GetQueued(int policyRequirement, int versionId)
        {
            int iAssignmentEnum = AssignmentEnum.Attachment.AsInt();

            var ids = await _Context.PolicyRequirementModificationAssignmentQueues.Where(e => e.PolicyRequirementId == policyRequirement && e.VersionId == versionId && e.AssignmentTypeId == iAssignmentEnum).Select(e => e.Id).Distinct().ToArrayAsync();

            var entities = await _Context.PolicyRequirementAttachments.Where(e => ids.Contains(e.ID)).ToArrayAsync();

            var models = entities.Select(e => MapToModel(e)).ToList();

            return models;
        }


        public async Task<int> AddToQueue(int Id, int policyRequirementId, int versionId)
        {
            int result = 0;
            int iAssignmentEnum = AssignmentEnum.Attachment.AsInt();

            var entity = await _Context.PolicyRequirementModificationAssignmentQueues.SingleOrDefaultAsync(e => e.Id == Id && e.PolicyRequirementId == policyRequirementId && e.VersionId == versionId && e.AssignmentTypeId == iAssignmentEnum);

            if (entity == null)
            {
                entity = new PolicyRequirementModificationAssignmentQueue();
                entity.AssignmentTypeId = iAssignmentEnum;
                entity.Id = Id;
                entity.PolicyRequirementId = policyRequirementId;
                entity.VersionId = versionId;

                _Context.PolicyRequirementModificationAssignmentQueues.AddObject(entity);

                result = 1;
            }

            return result;
        }

        public async Task<int> DeleteFromQueue(int Id, int policyRequirementId, int versionId)
        {
            int result = 0;

            int iAssignmentEnum = AssignmentEnum.Attachment.AsInt();

            var entity = await _Context.PolicyRequirementModificationAssignmentQueues.SingleOrDefaultAsync(e => e.Id == Id && e.PolicyRequirementId == policyRequirementId && e.VersionId == versionId && e.AssignmentTypeId == iAssignmentEnum);

            if (entity != null)
            {
                _Context.PolicyRequirementModificationAssignmentQueues.DeleteObject(entity);
                result = 1;
            }

            return result;
        }
    }
}
