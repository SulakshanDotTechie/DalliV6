using DALI.PolicyRequirements.DataEntityModels;
using DALI.PolicyRequirements.DomainModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.Repositories
{
    public class PolicyRequirementRepository : Repository
    {
        public PolicyRequirementRepository(IPolicyRequirementDataEntityModelContext context) : base(context)
        {
        }

        public async Task<List<PolicyRequirementModel>> GetAll(int versionId)
        {
            var models = await GetPolicyRequirements(versionId);

            return models;
        }

        public async Task<List<PolicyRequirementModel>> GetAll(int[] ids, int versionId)
        {
            var models = await GetPolicyRequirements(ids, versionId);

            return models;
        }

        public async Task<int[]> GetByAttachments(int versionId, string description)
        {
            var ids = await _Context.PolicyRequirementAssignedAttachments.Where(e => e.IsActive == 1 && e.VersionId == versionId && (e.PolicyRequirementAttachment != null && (e.PolicyRequirementAttachment.Description.Contains(description) || e.PolicyRequirementAttachment.StorageLocation.Contains(description)))).Select(e => e.PolicyRequirementId).Distinct().ToArrayAsync();

            return ids;
        }

        public async Task<int[]> GetBySourceReferences(int versionId, string description)
        {
            var result = await _Context.PolicyRequirementAssignedSources.Where(e => e.IsActive == 1 && e.VersionId == versionId && (e.PolicyRequirementSource != null && (e.PolicyRequirementSource.Description.Contains(description) || e.PolicyRequirementSource.StorageLocation.Contains(description)))).ToArrayAsync();

            var ids = result.Select(e => e.PolicyRequirementId).Distinct().ToArray();

            return ids;
        }

        protected async Task<PolicyRequirement> GetOneEntityAsync(int id, int versionId)
        {
            var entity = await _Context.PolicyRequirements.SingleOrDefaultAsync(e => e.ID == id && e.VersionId == versionId);

            return entity;
        }

        public async Task<PolicyRequirementModel> GetOneModelAsync(int id, int versionId)
        {
            var entity = await GetOneEntityAsync(id, versionId);

            if (entity != null)
            {
                var model = MapToModel(entity);
                return model;
            }

            return default(PolicyRequirementModel);
        }

        public async Task<int> Add(PolicyRequirementModel model)
        {
            int result = 0;

            var entity = MapToEntity(model);

            int nextId = await GetNextIdForEntity<PolicyRequirement>(entity);

            entity.ID = nextId;

            _Context.PolicyRequirements.AddObject(entity);

            model.Id = nextId;

            result = 1;

            return result;
        }

        public int AddWithId(PolicyRequirementModel model)
        {
            int result = 0;

            var entity = MapToEntity(model);

            entity.ID = model.Id;

            _Context.PolicyRequirements.AddObject(entity);

            result = 1;

            return result;
        }

        public async Task<int> Update(PolicyRequirementModel model)
        {
            int result = 0;

            var entity = await GetOneEntityAsync(model.Id, model.VersionId);

            if (entity != null)
            {
                entity.Description = model.Description;

                entity.ModifiedBy = model.ModifiedBy;
                entity.ModifiedDate = model.ModifiedDate;

                entity.VersionId = model.VersionId;

                result = 1;
            }

            return result;
        }

        public async Task<int> Delete(int id, int versionId)
        {
            int result = 0;

            var entity = await GetOneEntityAsync(id, versionId);

            if (entity != null)
            {
                _Context.PolicyRequirements.DeleteObject(entity);

                result = 1;
            }

            return result;
        }

        public async Task<int> UpdateOrder(int id, int orderIndex, int versionId, string userName)
        {
            var result = 0;

            var entity = await _Context.PolicyRequirements.Where(e => e.ID == id && e.VersionId == versionId).SingleOrDefaultAsync();

            if (entity != null)
            {
                if (string.Compare(entity.Owner, userName, true) != 0)
                    return -1;

                entity.OrderIndex = orderIndex;

                result = 1;
            }

            return result;
        }

        public void AssignSeverity(PolicyRequirementModel model, int severityId, int versionId)
        {
            var entity = new PolicyRequirementAssignedHardness();
            entity.PolicyRequirementId = model.Id;
            entity.HardnessId = severityId;
            entity.VersionId = versionId;
            entity.IsActive = 1;
            _Context.PolicyRequirementAssignedHardnesses.AddObject(entity);
        }

        public void AssignSourceDocument(PolicyRequirementModel model, int sourceId, int versionId)
        {
            var entity = new PolicyRequirementAssignedSource();
            entity.PolicyRequirementId = model.Id;
            entity.SourceId = sourceId;
            entity.VersionId = versionId;
            entity.IsActive = 1;
            _Context.PolicyRequirementAssignedSources.AddObject(entity);
        }

        public void AssignAttachment(PolicyRequirementModel model, int attachmentId, int versionId)
        {
            var entity = new PolicyRequirementAssignedAttachment();
            entity.PolicyRequirementId = model.Id;
            entity.AttachmentId = attachmentId;
            entity.IsActive = 1;
            entity.VersionId = versionId;
            _Context.PolicyRequirementAssignedAttachments.AddObject(entity);
        }

        public async Task<List<int>> AssignNewOwner(int chapterId, string oldOwner, string newOwner, int versionId)
        {
            List<int> ids = new List<int>();

            var entities = await _Context.PolicyRequirements.Where(e => string.Compare(e.Owner, newOwner, true) != 0 && e.ChapterID == chapterId && e.VersionId == versionId).ToArrayAsync();

            foreach (var entity in entities)
            {
                entity.Owner = newOwner;

                ids.Add(entity.ID);
            }

            return ids;
        }
    }
}
