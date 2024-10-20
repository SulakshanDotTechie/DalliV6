using DALI.DesignBrief.DataEntityModels;
using DALI.DesignBrief.DomainModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.DesignBrief.Repositories
{
    public class DesignBriefAreaRepository : Repository
    {
        public DesignBriefAreaRepository(IDesignBriefDataEntityModelContext context) : base(context)
        {

        }

        private DesignBriefPolicyRequirementArea MapToEntity(DesignBriefAreaModel model)
        {
            DesignBriefPolicyRequirementArea entity = new DesignBriefPolicyRequirementArea();

            entity.ID = model.Id;
            entity.VersionId = model.VersionId;
            entity.Description = model.Description;
            entity.DesignBriefId = model.DesignBriefId;

            return entity;
        }

        private DesignBriefAreaModel MapToModel(DesignBriefPolicyRequirementArea entity)
        {
            DesignBriefAreaModel model = new DesignBriefAreaModel();

            model.Id = entity.ID;
            model.VersionId = entity.VersionId;
            model.Description = entity.Description;
            model.DesignBriefId = entity.DesignBriefId;

            return model;
        }

        private IQueryable<DesignBriefPolicyRequirementArea> GetAllEntities()
        {
            IQueryable<DesignBriefPolicyRequirementArea> entities = _Context.DesignBriefPolicyRequirementAreas;

            return entities;
        }

        private IQueryable<DesignBriefAreaModel> AsModels(IQueryable<DesignBriefPolicyRequirementArea> entities)
        {
            IQueryable<DesignBriefAreaModel> models = entities.Select(e => new DesignBriefAreaModel()
            {
                Id = e.ID,
                VersionId = e.VersionId,
                Description = e.Description,
                DesignBriefId = e.DesignBriefId
            });

            return models;
        }

        private async Task<DesignBriefPolicyRequirementArea> GetOneEntityAsync(int designBriefId, int id)
        {
            DesignBriefPolicyRequirementArea entity = await GetAllEntities().SingleOrDefaultAsync(e => e.ID == id && e.DesignBriefId == designBriefId);

            return entity;
        }


        public async Task<DesignBriefAreaModel[]> GetBy(int designBriefId)
        {
            IOrderedQueryable<DesignBriefPolicyRequirementArea> entities = GetAllEntities().Where(e => e.DesignBriefId == designBriefId).OrderBy(e => e.Description);

            var models = await AsModels(entities).ToArrayAsync();

            return models;
        }

        public async Task<int> Add(DesignBriefAreaModel model)
        {
            int result = 0;

            using (DesignBriefDataEntityModels context = NewContext())
            {
                var entity = MapToEntity(model);

                context.DesignBriefPolicyRequirementAreas.AddObject(entity);

                await context.SaveChangesAsync();

                model.Id = entity.ID;

                result = 1;
            }

            return result;
        }

        public async Task<int> Update(DesignBriefAreaModel model)
        {
            int result = 0;

            DesignBriefPolicyRequirementArea entity = await GetOneEntityAsync(model.DesignBriefId, model.Id);

            if (entity != null)
            {
                entity.Description = model.Description;

                result = 1;
            }

            return result;
        }

        public async Task<int> Delete(int designBriefId, int id)
        {
            int result = 0;

            DesignBriefPolicyRequirementArea entity = await GetOneEntityAsync(designBriefId, id);

            if (entity != null)
            {
                _Context.DesignBriefPolicyRequirementAreas.DeleteObject(entity);

                result = 1;
            }

            return result;
        }
    }
}
