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
    public class PolicyRequirementChapterRepository : Repository
    {
        public PolicyRequirementChapterRepository(IPolicyRequirementDataEntityModelContext context) : base(context)
        {
        }

        private PolicyRequirementChapter MapToEntity(PolicyRequirementChapterModel model)
        {
            var entity = new PolicyRequirementChapter();

            entity.ID = model.Id;
            entity.Description = model.Description;
            entity.Owner = model.Owner;
            entity.IsActive = model.Active ? 1 : 0;
            entity.VersionId = model.VersionId;
            entity.ChapterNumber = model.ChapterNumber;
            entity.FetchByDefault = model.FetchByDefault ? 1 : 0;
            entity.IsTownSpecific = model.IsTownSpecific;

            return entity;
        }

        private PolicyRequirementChapterModel MapToModel(PolicyRequirementChapter entity)
        {
            var model = new PolicyRequirementChapterModel();

            // Fill model properties
            model.Id = entity.ID;
            model.ChapterNumber = entity.ChapterNumber;
            model.Active = (entity.IsActive == 1);
            model.Owner = entity.Owner;
            model.Description = entity.Description;
            model.VersionId = entity.VersionId;
            model.FetchByDefault = entity.FetchByDefault == 1;
            model.IsTownSpecific = entity.IsTownSpecific == true;

            return model;
        }

        private async Task<PolicyRequirementChapter> GetOneEntityAsync(int id, int versionId)
        {
            var entity = await _Context.PolicyRequirementChapters.SingleOrDefaultAsync(e => e.ID == id && (e.VersionId == versionId || e.VersionId == -1));

            return entity;
        }

        public async Task<List<PolicyRequirementChapterModel>> GetAll(int versionId)
        {
            var entities = await _Context.PolicyRequirementChapters.Where(e => e.VersionId == versionId && e.IsActive == 1).ToArrayAsync();

            var models = entities.Select(e => MapToModel(e)).ToList();

            return models;
        }

        public async Task<List<PolicyRequirementChapterModel>> GetForEditors(int versionId)
        {
            var entities = await _Context.PolicyRequirementChapters.Where(e => e.VersionId == versionId || e.VersionId == -1).ToArrayAsync();

            var models = entities.Select(e => MapToModel(e)).ToList();

            return models;
        }

        public async Task<List<PolicyRequirementChapterModel>> GetAll(string owner, int versionId)
        {
            var entities = await _Context.PolicyRequirementChapters.Where(e => string.Compare(e.Owner, owner, true) == 0 && (e.VersionId == versionId || e.VersionId == -1)).ToArrayAsync();

            var models = entities.Select(e => MapToModel(e)).ToList();

            return models;
        }

        public async Task<PolicyRequirementChapterModel> GetOneModelAsync(int id, int versionId)
        {
            var entity = await GetOneEntityAsync(id, versionId);

            if (entity != null)
            {
                var model = MapToModel(entity);

                return model;
            }

            return default(PolicyRequirementChapterModel);
        }

        private async Task<bool> Exists(string name, int versionId, IPolicyRequirementDataEntityModelContext context)
        {
            var entity = await context.PolicyRequirementChapters.SingleOrDefaultAsync(e => string.Compare(name, e.Description, true) == 0 && e.VersionId == versionId);

            return (entity != null);
        }

        public Task<bool> Exists(string name, int versionId)
        {
            return Exists(name, versionId, _Context);
        }

        public void Add(PolicyRequirementChapterModel model)
        {
            PolicyRequirementChapter entity = MapToEntity(model);

            _Context.PolicyRequirementChapters.AddObject(entity);
        }

        public async Task<int> Add(PolicyRequirementChapterModel model, int version)
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

                var entity = MapToEntity(model);

                int nextId = 1;

                var ents = await context.PolicyRequirementChapters.Where(e => e.VersionId == -1).ToArrayAsync();

                if (ents.Length == 0)
                {
                    ents = await context.PolicyRequirementChapters.Where(e => e.VersionId == version).ToArrayAsync();
                }

                if (ents.Length > 0)
                {
                    var lastEnt = ents.OrderByDescending(e => e.ID).First();

                    if (lastEnt != null)
                    {
                        nextId = lastEnt.ID + 1;
                    }
                }

                entity.ID = nextId;

                entity.ChapterNumber = (entity.ID < 10 ? string.Format("0{0}", entity.ID) : entity.ID.ToString());

                context.PolicyRequirementChapters.AddObject(entity);

                await context.SaveChangesAsync();

                model.Id = nextId;
                model.ChapterNumber = entity.ChapterNumber;

                result = 1;
            }

            return result;
        }

        public async Task<int> Update(PolicyRequirementChapterModel model, int version)
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

                var entity = await context.PolicyRequirementChapters.SingleOrDefaultAsync(e => e.ID == model.Id && e.VersionId == version);

                if (entity != null)
                {
                    entity.ChapterNumber = model.ChapterNumber;
                    entity.Description = model.Description;
                    entity.IsActive = model.Active ? 1 : 0;
                    entity.VersionId = model.VersionId;
                    entity.FetchByDefault = model.FetchByDefault ? 1 : 0;
                    entity.IsTownSpecific = model.IsTownSpecific;

                    await context.SaveChangesAsync();

                    result = 1;
                }
            }

            return result;
        }

        public async Task<int> UpdateOwner(PolicyRequirementChapterModel model)
        {
            int result = 0;

            var entity = await GetOneEntityAsync(model.Id, model.VersionId);

            if (entity != null)
            {
                entity.Owner = model.Owner;

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
                _Context.PolicyRequirementChapters.DeleteObject(entity);

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
