using DALI.PolicyRequirements.DataEntityModels;
using DALI.PolicyRequirements.DomainModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.Repositories
{
    public class PolicyRequirementLevelRepository : Repository
    {
        public PolicyRequirementLevelRepository(IPolicyRequirementDataEntityModelContext context) : base(context)
        {
        }

        private PolicyRequirementLevel MapToEntity(PolicyRequirementLevelModel model)
        {
            var entity = new PolicyRequirementLevel();

            entity.ID = model.Id;
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.Position = model.Position;
            entity.IsActive = model.Active ? 1 : 0;

            return entity;
        }

        private PolicyRequirementLevelModel MapToModel(PolicyRequirementLevel entity)
        {
            var model = new PolicyRequirementLevelModel();

            // Fill model properties
            model.Id = entity.ID;
            model.Name = entity.Name;
            model.Active = (entity.IsActive == 1);
            model.Description = entity.Description;
            model.Position = entity.Position;

            return model;
        }

        private async Task<PolicyRequirementLevel> GetOneEntityAsync(int id)
        {
            var entity = await _Context.PolicyRequirementLevels.SingleOrDefaultAsync(e => e.ID == id);

            return (PolicyRequirementLevel)entity;
        }

        public async Task<int> Add(PolicyRequirementLevelModel model)
        {
            int result = 0;
            using (var context = NewContext())
            {
                var entity = MapToEntity(model);

                int nextId = await GetNextIdForEntity(new PolicyRequirementLevel());

                entity.ID = nextId;

                context.PolicyRequirementLevels.AddObject(entity);
                await context.SaveChangesAsync();

                model.Id = nextId;
            }

            result = 1;

            return result;
        }

        public async Task<List<PolicyRequirementLevelModel>> GetAll()
        {
            var models = new List<PolicyRequirementLevelModel>();

            var entities = await _Context.PolicyRequirementLevels.ToListAsync();

            foreach (var entity in entities)
            {
                var model = MapToModel(entity);
                models.Add(model);
            }

            return models;
        }

        public async Task<PolicyRequirementLevelModel> GetOneModelAsync(int id)
        {
            var entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                var model = MapToModel(entity);
                return model;
            }

            return default(PolicyRequirementLevelModel);
        }

        public async Task<int> Update(PolicyRequirementLevelModel model)
        {
            int result = 0;

            var entity = await GetOneEntityAsync(model.Id);

            if (entity != null)
            {
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Position = model.Position;
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
                _Context.PolicyRequirementLevels.DeleteObject(entity);

                result = 1;
            }

            return result;
        }

        public async Task<List<PolicyRequirementLevelPropertyModel>> GetProperties(int levelId)
        {
            List<PolicyRequirementLevelPropertyModel> models = new List<PolicyRequirementLevelPropertyModel>();

            var entities = await _Context.PolicyRequirementLevels.ToListAsync();

            var entity = entities.SingleOrDefault(e => e.ID == levelId);

            if (null != entity)
            {
                foreach (var p in entity.PolicyRequirementLevelProperties)
                {
                    var model = new PolicyRequirementLevelPropertyModel()
                    {
                        Id = p.Id,
                        Description = p.Description,
                        Sequence = p.Sequence
                    };

                    models.Add(model);
                }

                return models.OrderBy(p => p.Sequence).ToList();
            }

            return models;
        }
    }
}
