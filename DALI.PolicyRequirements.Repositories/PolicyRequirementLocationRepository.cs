using DALI.PolicyRequirements.DataEntityModels;
using DALI.PolicyRequirements.DomainModels;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.Repositories
{
    public class PolicyRequirementLocationRepository : Repository
    {
        public PolicyRequirementLocationRepository(IPolicyRequirementDataEntityModelContext context) : base(context)
        {
        }

        private PolicyRequirementLocation MapToEntity(PolicyRequirementLocationModel model)
        {
            var entity = new PolicyRequirementLocation();

            entity.Id = model.Id;
            entity.Description = model.Description;
            entity.FetchByDefault = model.FetchByDefault;
            entity.OrderIndex = model.OrderIndex;

            return entity;
        }

        private PolicyRequirementLocationModel MapToModel(PolicyRequirementLocation entity)
        {
            var model = new PolicyRequirementLocationModel();

            // Fill model properties
            model.Id = entity.Id;
            model.Description = entity.Description;
            model.FetchByDefault = entity.FetchByDefault ?? default(bool);
            model.OrderIndex = entity.OrderIndex ?? default(int);

            return model;
        }

        private async Task<PolicyRequirementLocation> GetOneEntityAsync(Guid id)
        {
            var entity = await _Context.PolicyRequirementLocations.SingleOrDefaultAsync(e => e.Id == id);

            return entity;
        }

        public async Task<int> Add(PolicyRequirementLocationModel model)
        {
            int result = 0;

            var entity = await GetOneEntityAsync(model.Id);

            if (entity == null)
            {
                entity = MapToEntity(model);

                _Context.PolicyRequirementLocations.AddObject(entity);

                model.Id = entity.Id;

                result = 1;
            }

            return result;
        }

        public async Task<Guid[]> GetFetchByDefaultIds()
        {
            var ids = await _Context.PolicyRequirementLocations.Where(e => e.FetchByDefault == true).Select(e => e.Id).Distinct().ToArrayAsync();

            return ids;
        }

        public async Task<List<PolicyRequirementLocationModel>> GetAll()
        {
            var entities = await _Context.PolicyRequirementLocations.ToArrayAsync();

            var models = entities.OrderBy(e => e.OrderIndex).Select(e => MapToModel(e)).ToList();

            return models;
        }

        public async Task<PolicyRequirementLocationModel> GetOneModelAsync(Guid id)
        {
            var entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                var model = MapToModel(entity);

                return model;
            }

            return default(PolicyRequirementLocationModel);
        }

        public async Task<int> Update(PolicyRequirementLocationModel model)
        {
            int result = 0;

            var entity = await GetOneEntityAsync(model.Id);

            if (entity != null)
            {
                entity.Description = model.Description;

                result = 1;
            }

            return result;
        }

        public async Task<int> Delete(Guid id)
        {
            int result = 0;

            var entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                _Context.PolicyRequirementLocations.DeleteObject(entity);

                result = 1;
            }

            return result;
        }
    }
}
