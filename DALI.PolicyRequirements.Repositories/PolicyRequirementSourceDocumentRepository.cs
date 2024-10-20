using DALI.PolicyRequirements.DataEntityModels;
using DALI.PolicyRequirements.DomainModels;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALI.Enums;

namespace DALI.PolicyRequirements.Repositories
{
    public class PolicyRequirementSourceDocumentRepository : Repository
    {
        public PolicyRequirementSourceDocumentRepository(IPolicyRequirementDataEntityModelContext context) : base(context)
        {
        }

        private PolicyRequirementSource MapToEntity(PolicyRequirementSourceDocumentModel model)
        {
            var entity = new PolicyRequirementSource();

            entity.ID = model.Id;
            entity.Description = model.Description;
            entity.StorageLocation = model.StorageLocation;
            entity.IsActive = model.Active ? 1 : 0;

            return entity;
        }

        private PolicyRequirementSourceDocumentModel MapToModel(PolicyRequirementSource entity)
        {
            var model = new PolicyRequirementSourceDocumentModel();

            // Fill model properties
            model.Id = entity.ID;
            model.Active = (entity.IsActive == 1);
            model.Description = entity.Description;
            model.StorageLocation = (!string.IsNullOrEmpty(entity.StorageLocation) && entity.StorageLocation.Contains(_FileFolder)) ? entity.StorageLocation.Replace("~/", _BaseUrl) : entity.StorageLocation;
            model.BaseLocation = entity.StorageLocation;
            model.FileExtension = System.IO.Path.GetExtension(model.StorageLocation);

            return model;
        }

        private async Task<PolicyRequirementSource> GetOneEntityAsync(int id)
        {
            var entity = await _Context.PolicyRequirementSources.SingleOrDefaultAsync(e => e.ID == id);

            return (PolicyRequirementSource)entity;
        }

        public async Task<int> Add(PolicyRequirementSourceDocumentModel model)
        {
            int result = 0;

            var entity = MapToEntity(model);

            int nextId = await GetNextIdForEntity(entity);

            entity.ID = nextId;

            _Context.PolicyRequirementSources.AddObject(entity);

            await _Context.SaveChangesAsync();

            model.Id = nextId;

            result = 1;

            return result;
        }

        public int AddWithInitializedId(PolicyRequirementSourceDocumentModel model)
        {
            int result = 0;

            var entity = MapToEntity(model);

            _Context.PolicyRequirementSources.AddObject(entity);

            result = 1;

            return result;
        }

        public async Task<List<PolicyRequirementSourceDocumentModel>> GetAll()
        {
            var models = new List<PolicyRequirementSourceDocumentModel>();

            var entities = await _Context.PolicyRequirementSources.OrderBy(e => e.Description).ToListAsync();

            foreach (var entity in entities)
            {
                var model = MapToModel(entity);
                models.Add(model);
            }

            return models;
        }

        public async Task<List<PolicyRequirementSourceDocumentModel>> GetAssigned(int versionId)
        {
            var models = new List<PolicyRequirementSourceDocumentModel>();

            var ids = await _Context.PolicyRequirementAssignedSources.Where(e => e.VersionId == versionId && e.IsActive == 1).Select(e => e.SourceId).Distinct().ToArrayAsync();

            var entities = await _Context.PolicyRequirementSources.Where(e => ids.Contains(e.ID)).OrderBy(e => e.Description).ToArrayAsync();

            foreach (var entity in entities)
            {
                var model = MapToModel(entity);

                model.IsAssigned = true;
                model.PolicyRequirementId = entity.ID;
                model.VersionId = versionId;

                models.Add(model);
            }

            return models;
        }

        public async Task<List<PolicyRequirementSourceDocumentModel>> GetAssigned(int policyRequirementId, int versionId)
        {

            var models = new List<PolicyRequirementSourceDocumentModel>();

            var entities = await _Context.PolicyRequirementAssignedSources.Where(e => e.PolicyRequirementId == policyRequirementId && e.VersionId == versionId && e.IsActive == 1).ToArrayAsync();

            foreach (var entity in entities)
            {
                if (entity.PolicyRequirementSource == null)
                    throw new Exception(string.Format("SourceReferenceError: {0} - {1}", entity.PolicyRequirementId, entity.SourceId));

                var model = MapToModel(entity.PolicyRequirementSource);

                model.IsAssigned = true;
                model.PolicyRequirementId = entity.PolicyRequirementId;
                model.VersionId = entity.VersionId;

                models.Add(model);
            }

            return models;
        }

        public async Task<PolicyRequirementSourceDocumentModel> GetOneModelAsync(int id)
        {
            var entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                var model = MapToModel(entity);
                return model;
            }

            return default(PolicyRequirementSourceDocumentModel);
        }

        public async Task<PolicyRequirementSourceDocumentModel> GetOneModelAsync(string description)
        {
            var entity = await _Context.PolicyRequirementSources.SingleOrDefaultAsync(e => !string.IsNullOrEmpty(e.Description) && (string.Compare(e.Description, description, true) == 0));

            if (entity != null)
            {
                var model = MapToModel(entity);
                return model;
            }

            return default(PolicyRequirementSourceDocumentModel);
        }

        public async Task<int> Update(PolicyRequirementSourceDocumentModel model)
        {
            int result = 0;

            var entity = await GetOneEntityAsync(model.Id);

            if (entity != null)
            {
                entity.Description = model.Description;
                entity.IsActive = (model.Active ? 1 : 0);

                result = 1;
            }

            return result;
        }

        public async Task<int> Delete(int id)
        {
            int result = 0;

            var entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                _Context.PolicyRequirementSources.DeleteObject(entity);

                result = 1;
            }

            return result;
        }

        public async Task<List<PolicyRequirementSourceDocumentModel>> GetQueued(int versionId)
        {
            var models = new List<PolicyRequirementSourceDocumentModel>();
            int iAssignmentSourceEnum = AssignmentEnum.SourceDocument.AsInt();

            var ids = await _Context.PolicyRequirementModificationAssignmentQueues.Where(e => e.VersionId == versionId && e.AssignmentTypeId == iAssignmentSourceEnum).Select(e => e.Id).Distinct().ToArrayAsync();
            var entities = await _Context.PolicyRequirementSources.Where(e => ids.Contains(e.ID)).ToListAsync();

            foreach (var entity in entities)
            {
                var model = MapToModel(entity);
                models.Add(model);
            }

            return models;
        }

        public async Task<List<PolicyRequirementSourceDocumentModel>> GetQueued(int policyRequirement, int versionId)
        {
            var models = new List<PolicyRequirementSourceDocumentModel>();

            int iAssignmentEnum = AssignmentEnum.SourceDocument.AsInt();

            var queuedRefEntities = await _Context.PolicyRequirementModificationAssignmentQueues.Where(e => e.PolicyRequirementId == policyRequirement && e.VersionId == versionId && e.AssignmentTypeId == iAssignmentEnum).ToArrayAsync();

            foreach (var qEntity in queuedRefEntities)
            {
                var entity = await _Context.PolicyRequirementSources.SingleOrDefaultAsync(e => e.ID == qEntity.Id);

                if (entity != null)
                {
                    var model = MapToModel(entity);

                    model.IsAssigned = true;
                    model.PolicyRequirementId = qEntity.PolicyRequirementId;
                    model.VersionId = qEntity.VersionId;

                    models.Add(model);
                }
            }

            return models;
        }

        public async Task<int> AddToQueue(int Id, int policyRequirementId, int versionId)
        {
            int result = 0;
            int iAssignmentSourceEnum = AssignmentEnum.SourceDocument.AsInt();

            var entity = await _Context.PolicyRequirementModificationAssignmentQueues.SingleOrDefaultAsync(e => e.Id == Id && e.PolicyRequirementId == policyRequirementId && e.VersionId == versionId && e.AssignmentTypeId == iAssignmentSourceEnum);

            if (entity == null)
            {
                entity = new PolicyRequirementModificationAssignmentQueue();
                entity.AssignmentTypeId = iAssignmentSourceEnum;
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
            int iAssignmentSourceEnum = AssignmentEnum.SourceDocument.AsInt();

            var entity = await _Context.PolicyRequirementModificationAssignmentQueues.SingleOrDefaultAsync(e => e.Id == Id && e.PolicyRequirementId == policyRequirementId && e.VersionId == versionId && e.AssignmentTypeId == iAssignmentSourceEnum);

            if (entity != null)
            {
                _Context.PolicyRequirementModificationAssignmentQueues.DeleteObject(entity);
                result = 1;
            }

            return result;
        }
    }
}
