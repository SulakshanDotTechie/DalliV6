using DALI.Enums;
using DALI.PolicyRequirements.DataEntityModels;
using DALI.PolicyRequirements.DomainModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.Repositories
{
    public class PolicyRequirementSeverityRepository : Repository
    {
        public PolicyRequirementSeverityRepository(IPolicyRequirementDataEntityModelContext context) : base(context)
        {
        }

        private PolicyRequirementHardness MapToEntity(PolicyRequirementSeverityModel model)
        {
            var entity = new PolicyRequirementHardness();

            entity.ID = model.Id;
            entity.ShortName = model.ShortName;
            entity.Description = model.Description;
            entity.IsActive = model.Active ? 1 : 0;
            entity.HelpText = model.Clarification;

            return entity;
        }

        private PolicyRequirementSeverityModel MapToModel(PolicyRequirementHardness entity)
        {
            var model = new PolicyRequirementSeverityModel();

            // Fill model properties
            model.Id = entity.ID;
            model.ShortName = entity.ShortName;
            model.Description = entity.Description;
            model.Active = (entity.IsActive == 1);
            model.Clarification = entity.HelpText;

            return model;
        }

        private async Task<PolicyRequirementHardness> GetOneEntityAsync(int id)
        {
            var entity = await _Context.PolicyRequirementHardnesses.SingleOrDefaultAsync(e => e.ID == id);

            return entity;
        }

        public async Task<int> Add(PolicyRequirementSeverityModel model)
        {
            int result = 0;

            var entity = MapToEntity(model);

            int nextId = await GetNextIdForEntity(new PolicyRequirementHardness());

            entity.ID = nextId;

            _Context.PolicyRequirementHardnesses.AddObject(entity);

            model.Id = nextId;

            result = 1;

            return result;
        }

        public async Task<List<PolicyRequirementSeverityModel>> GetAll()
        {
            var models = new List<PolicyRequirementSeverityModel>();

            var entities = await _Context.PolicyRequirementHardnesses.OrderBy(e => e.Description).ToArrayAsync();

            foreach (var entity in entities)
            {
                var model = MapToModel(entity);
                models.Add(model);
            }

            return models;
        }

        public async Task<List<PolicyRequirementSeverityModel>> GetAssigned(int versionId)
        {
            var models = new List<PolicyRequirementSeverityModel>();

            var entities = await _Context.PolicyRequirementAssignedHardnesses.Where(e => e.VersionId == versionId).ToArrayAsync();

            foreach (var entity in entities)
            {
                var model = MapToModel(entity.PolicyRequirementHardness);

                model.IsAssigned = true;
                model.PolicyRequirementId = entity.PolicyRequirementId;
                model.VersionId = entity.VersionId;

                models.Add(model);
            }

            return models;
        }

        public async Task<List<PolicyRequirementSeverityModel>> GetAssigned(int policyRequirementId, int versionId)
        {
            var models = new List<PolicyRequirementSeverityModel>();

            var entities = await _Context.PolicyRequirementAssignedHardnesses.Where(e => e.PolicyRequirementId == policyRequirementId && e.VersionId == versionId).ToArrayAsync();

            foreach (var entity in entities)
            {
                var model = MapToModel(entity.PolicyRequirementHardness);

                model.IsAssigned = true;
                model.PolicyRequirementId = entity.PolicyRequirementId;
                model.VersionId = entity.VersionId;
                model.AdditionalInfo = entity.HelpText;

                models.Add(model);
            }

            return models;
        }

        public async Task<PolicyRequirementSeverityModel> GetOneModelAsync(int id)
        {
            var entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                var model = MapToModel(entity);
                return model;
            }

            return default(PolicyRequirementSeverityModel);
        }

        public async Task<int> Update(PolicyRequirementSeverityModel model)
        {
            int result = 0;

            var entity = await GetOneEntityAsync(model.Id);

            if (entity != null)
            {
                entity.Description = model.Description;
                entity.IsActive = (model.Active ? 1 : 0);
                entity.HelpText = model.Clarification;

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
                _Context.PolicyRequirementHardnesses.DeleteObject(entity);

                result = 1;
            }

            return result;
        }

        public async Task<List<PolicyRequirementSeverityModel>> GetQueued(int policyRequirementId, int versionId)
        {
            var models = new List<PolicyRequirementSeverityModel>();
            int iAssignmentEnum = AssignmentEnum.Strictness.AsInt();

            var queuedRefEntities = await _Context.PolicyRequirementModificationAssignmentQueues.Where(e => e.PolicyRequirementId == policyRequirementId && e.VersionId == versionId && e.AssignmentTypeId == iAssignmentEnum).ToArrayAsync();

            foreach (var qEntity in queuedRefEntities)
            {
                var entity = await _Context.PolicyRequirementHardnesses.SingleOrDefaultAsync(e => e.ID == qEntity.Id);
                if (entity != null)
                {
                    var model = MapToModel(entity);
                    model.PolicyRequirementId = policyRequirementId;
                    model.VersionId = versionId;
                    models.Add(model);
                }
            }

            return models;
        }


        public async Task<int> AddToQueue(int Id, int policyRequirementId, int versionId)
        {
            int result = 0;
            int iAssignmentEnum = AssignmentEnum.Strictness.AsInt();

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
            int iAssignmentEnum = AssignmentEnum.Strictness.AsInt();

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
