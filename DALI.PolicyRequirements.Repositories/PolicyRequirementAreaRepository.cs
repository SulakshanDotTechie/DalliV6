using DALI.PolicyRequirements.DataEntityModels;
using DALI.PolicyRequirements.DomainModels;
using DALI.PolicyRequirements.Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.Repositories
{
    public class PolicyRequirementAreaRepository : Repository
    {
        public PolicyRequirementAreaRepository(IPolicyRequirementDataEntityModelContext context) : base(context)
        {
        }

        private PolicyRequirementArea MapToEntity(PolicyRequirementAreaModel model)
        {
            PolicyRequirementArea entity = new PolicyRequirementArea();

            entity.ID = model.Id;
            entity.Description = model.Description;
            entity.FetchByDefault = model.FetchByDefault ? 1 : 0;
            entity.IsTownSpecific = model.IsTownSpecific;
            entity.IsActive = model.Active ? 1 : 0;
            entity.VersionId = model.VersionId;

            return entity;
        }

        private PolicyRequirementAreaModel MapToModel(PolicyRequirementArea entity)
        {
            PolicyRequirementAreaModel model = new PolicyRequirementAreaModel();

            // Fill model properties
            model.Id = entity.ID;
            model.Active = (entity.IsActive == 1);
            model.Description = entity.Description;
            model.FetchByDefault = entity.FetchByDefault == 1;
            model.IsTownSpecific = entity.IsTownSpecific == true;
            model.VersionId = entity.VersionId;

            return model;
        }

        private async Task<PolicyRequirementArea> GetOneEntityAsync(int id, int versionId)
        {
            var entity = await _Context.PolicyRequirementAreas.SingleOrDefaultAsync(e => e.ID == id && (e.VersionId == versionId || e.VersionId == -1));

            return (PolicyRequirementArea)entity;
        }

        private async Task<bool> Exists(string name, int versionId, IPolicyRequirementDataEntityModelContext context)
        {
            var entity = await context.PolicyRequirementAreas.SingleOrDefaultAsync(e => string.Compare(name, e.Description, true) == 0 && e.VersionId == versionId);

            return (entity != null);
        }

        public Task<bool> Exists(string name, int versionId)
        {
            return Exists(name, versionId, _Context);
        }

        public async Task<int> Add(PolicyRequirementAreaModel model, int version)
        {
            int result = 0;

            using (var context = NewContext())
            {
                // Versioned already exists?
                var vExists = await Exists(model.Description, version, context);

                // NonVersioned already exists?
                var nvExists = await Exists(model.Description, -1, context);

                if (vExists || nvExists)
                    throw new Exception(Localization.ErrorDuplicateValue);

                model.Active = true;
                model.VersionId = -1;

                PolicyRequirementArea entity = MapToEntity(model);

                int nextId = await GetNextIdForEntity(entity, context);

                entity.ID = nextId;

                context.PolicyRequirementAreas.AddObject(entity);

                await context.SaveChangesAsync();

                model.Id = nextId;

                result = 1;
            }

            return result;
        }

        public int AddWithInitializedId(PolicyRequirementAreaModel model)
        {
            int result = 0;

            PolicyRequirementArea entity = MapToEntity(model);

            _Context.PolicyRequirementAreas.AddObject(entity);

            result = 1;

            return result;
        }

        public async Task<List<PolicyRequirementAreaModel>> GetAll(int versionId)
        {
            var entities = await _Context.PolicyRequirementAreas.Where(e => e.VersionId == versionId && e.IsActive == 1).ToArrayAsync();

            var models = entities.Select(e => MapToModel(e)).OrderBy(e => e.Description).ToList();

            return models;
        }

        public async Task<List<PolicyRequirementAreaModel>> GetAll(int versionId, int[] ids)
        {
            var entities = await _Context.PolicyRequirementAreas.Where(e => ids.Contains(e.ID) && e.VersionId == versionId && e.IsActive == 1).ToArrayAsync();

            var models = entities.Select(e => MapToModel(e)).OrderBy(e => e.Description).ToList();

            return models;
        }


        public async Task<List<PolicyRequirementAreaModel>> GetForEditors(int versionId)
        {
            var entities = await _Context.PolicyRequirementAreas.Where(e => e.VersionId == versionId || e.VersionId == -1).ToArrayAsync();

            var models = entities.Select(e => MapToModel(e)).OrderBy(e => e.Description).ToList();

            return models;
        }

        public async Task<PolicyRequirementAreaModel> GetOneModelAsync(int id, int versionId)
        {
            PolicyRequirementArea entity = await GetOneEntityAsync(id, versionId);

            if (entity != null)
            {
                var model = MapToModel(entity);

                return model;
            }

            return default(PolicyRequirementAreaModel);
        }


        public async Task<PolicyRequirementAreaModel> GetByDescription(string description, int versionId)
        {
            PolicyRequirementArea entity = await _Context.PolicyRequirementAreas.SingleOrDefaultAsync(e => string.Compare(e.Description, description, true) == 0 && (e.VersionId == versionId || e.VersionId == -1));

            if (entity != null)
            {
                var model = MapToModel(entity);
                return model;
            }

            return default(PolicyRequirementAreaModel);
        }

        public async Task<int> Update(PolicyRequirementAreaModel model, int version)
        {
            int result = 0;

            using (var context = NewContext())
            {
                // Versioned already exists?
                bool vExists = await Exists(model.Description, version, context);
                bool nvExists = false;

                if (model.VersionId > -1)
                {
                    // NonVersioned already exists?
                    nvExists = await Exists(model.Description, -1, _Context);
                }

                if (vExists || nvExists)
                    throw new Exception(Localization.ErrorDuplicateValue);

                var entity = await context.PolicyRequirementAreas.SingleOrDefaultAsync(e => e.ID == model.Id && e.VersionId == version);

                if (entity != null)
                {
                    entity.Description = model.Description;
                    entity.IsActive = (model.Active ? 1 : 0);
                    entity.VersionId = model.VersionId;
                    entity.IsTownSpecific = model.IsTownSpecific;

                    await context.SaveChangesAsync();

                    result = 1;
                }
            }

            return result;
        }

        public async Task<int> Delete(int id, int versionId)
        {
            int result = 0;

            PolicyRequirementArea entity = await GetOneEntityAsync(id, versionId);

            if (entity != null)
            {
                _Context.PolicyRequirementAreas.DeleteObject(entity);

                result = 1;
            }

            return result;
        }

        public async Task Refresh(int id, int version)
        {
            var entity = await GetOneEntityAsync(id, version);

            if (entity != null)
            {
                _Context.Refresh(RefreshMode.StoreWins, entity);
            }
        }
    }
}
