﻿using DALI.PolicyRequirements.DataEntityModels;
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
    public class PolicyRequirementSubjectRepository : Repository
    {
        public PolicyRequirementSubjectRepository(IPolicyRequirementDataEntityModelContext context) : base(context)
        {
        }

        private PolicyRequirementSubject MapToEntity(PolicyRequirementSubjectModel model)
        {
            var entity = new PolicyRequirementSubject();

            entity.ID = model.Id;
            entity.Description = model.Description;
            entity.IsActive = model.Active ? 1 : 0;
            entity.VersionId = model.VersionId;
            entity.FetchByDefault = model.FetchByDefault ? 1 : 0;
            entity.IsTownSpecific = model.IsTownSpecific;

            return entity;
        }

        private PolicyRequirementSubjectModel MapToModel(PolicyRequirementSubject entity)
        {
            var model = new PolicyRequirementSubjectModel();

            // Fill model properties
            model.Id = entity.ID;
            model.Active = (entity.IsActive == 1);
            model.Description = entity.Description;
            model.VersionId = entity.VersionId;
            model.FetchByDefault = entity.FetchByDefault == 1;
            model.IsTownSpecific = entity.IsTownSpecific == true;

            return model;
        }

        private async Task<PolicyRequirementSubject> GetOneEntityAsync(int id, int versionId)
        {
            var entity = await _Context.PolicyRequirementSubjects.SingleOrDefaultAsync(e => e.ID == id && (e.VersionId == versionId || e.VersionId == -1));

            return entity;
        }
        private async Task<bool> Exists(string name, int versionId, IPolicyRequirementDataEntityModelContext context)
        {
            var entity = await context.PolicyRequirementSubjects.SingleOrDefaultAsync(e => string.Compare(name, e.Description, true) == 0 && e.VersionId == versionId);

            return (entity != null);
        }

        public Task<bool> Exists(string name, int versionId)
        {
            return Exists(name, versionId, _Context);
        }

        public async Task<int> Add(PolicyRequirementSubjectModel model, int version)
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

                PolicyRequirementSubject entity = MapToEntity(model);

                int nextId = await GetNextIdForEntity(entity, context);

                entity.ID = nextId;

                context.PolicyRequirementSubjects.AddObject(entity);

                await context.SaveChangesAsync();

                model.Id = nextId;

                result = 1;
            }

            return result;
        }

        public async Task<PolicyRequirementSubjectModel> GetByDescription(string description, int versionId)
        {
            PolicyRequirementSubject entity = await _Context.PolicyRequirementSubjects.SingleOrDefaultAsync(e => string.Compare(e.Description, description, true) == 0 && (e.VersionId == versionId || e.VersionId == -1));

            if (entity != null)
            {
                var model = MapToModel(entity);
                return model;
            }

            return default(PolicyRequirementSubjectModel);
        }

        public async Task<PolicyRequirementSubjectModel> GetByDescriptionVersioned(string description, int versionId)
        {
            PolicyRequirementSubject entity = await _Context.PolicyRequirementSubjects.SingleOrDefaultAsync(e => string.Compare(e.Description, description, true) == 0 && (e.VersionId == versionId));

            if (entity != null)
            {
                var model = MapToModel(entity);
                return model;
            }

            return default(PolicyRequirementSubjectModel);
        }

        public int AddWithInitializedId(PolicyRequirementSubjectModel model)
        {
            int result = 0;

            var entity = MapToEntity(model);

            _Context.PolicyRequirementSubjects.AddObject(entity);

            result = 1;

            return result;
        }

        public async Task<List<PolicyRequirementSubjectModel>> GetAll(int versionId)
        {
            var entities = await _Context.PolicyRequirementSubjects.Where(e => e.VersionId == versionId && e.IsActive == 1).ToArrayAsync();

            var models = entities.Select(e => MapToModel(e)).OrderBy(e => e.Description).ToList();

            return models;
        }

        public async Task<List<PolicyRequirementSubjectModel>> GetAll(int versionId, int[] ids)
        {
            var entities = await _Context.PolicyRequirementSubjects.Where(e => ids.Contains(e.ID) && e.VersionId == versionId && e.IsActive == 1).ToArrayAsync();

            var models = entities.Select(e => MapToModel(e)).OrderBy(e => e.Description).ToList();

            return models;
        }

        public async Task<List<PolicyRequirementSubjectModel>> GetForEditors(int versionId)
        {
            var entities = await _Context.PolicyRequirementSubjects.Where(e => (e.VersionId == versionId || e.VersionId == -1) && e.IsActive == 1).ToArrayAsync();

            var models = entities.Select(e => MapToModel(e)).OrderBy(e => e.Description).ToList();

            return models;
        }

        public async Task<PolicyRequirementSubjectModel> GetOneModelAsync(int id, int versionId)
        {
            var entity = await GetOneEntityAsync(id, versionId);

            if (entity != null)
            {
                var model = MapToModel(entity);
                return model;
            }

            return default(PolicyRequirementSubjectModel);
        }

        public async Task<int> Update(PolicyRequirementSubjectModel model, int version)
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

                var entity = await context.PolicyRequirementSubjects.SingleOrDefaultAsync(e => e.ID == model.Id && e.VersionId == version);

                if (entity != null)
                {
                    entity.Description = model.Description;

                    await context.SaveChangesAsync();

                    result = 1;
                }
            }

            return result;
        }

        public async Task<int> Delete(int id, int versionId)
        {
            int result = 0;

            var entity = await GetOneEntityAsync(id, versionId);

            if (entity != null)
            {
                _Context.PolicyRequirementSubjects.DeleteObject(entity);

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
