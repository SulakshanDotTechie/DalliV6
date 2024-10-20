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
    public class PolicyRequirementLocalAuthorityRepository : Repository
    {
        public PolicyRequirementLocalAuthorityRepository(IPolicyRequirementDataEntityModelContext context) : base(context)
        {
        }

        private PolicyRequirementLocalAuthority MapToEntity(PolicyRequirementLocalAuthorityModel model)
        {
            var entity = new PolicyRequirementLocalAuthority();

            entity.Id = model.Id;
            entity.Description = model.Description;

            return entity;
        }

        private PolicyRequirementLocalAuthorityModel MapToModel(PolicyRequirementLocalAuthority entity)
        {
            var model = new PolicyRequirementLocalAuthorityModel();

            // Fill model properties
            model.Id = entity.Id;
            model.Description = entity.Description;

            return model;
        }

        private async Task<PolicyRequirementLocalAuthority> GetOneEntityAsync(Guid id)
        {
            var entity = await _Context.PolicyRequirementLocalAuthorities.SingleOrDefaultAsync(e => e.Id == id);

            return entity;
        }

        public async Task<int> Add(PolicyRequirementLocalAuthorityModel model)
        {
            int result = 0;

            var entity = await GetOneEntityAsync(model.Id);

            if (entity == null)
            {
                entity = MapToEntity(model);

                _Context.PolicyRequirementLocalAuthorities.AddObject(entity);

                model.Id = entity.Id;

                result = 1;
            }

            return result;
        }

        public async Task<List<PolicyRequirementLocalAuthorityModel>> GetAll()
        {
            var entities = await _Context.PolicyRequirementLocalAuthorities.ToArrayAsync();

            var models = entities.Select(e => MapToModel(e)).ToList();

            return models;
        }


        public async Task<PolicyRequirementLocalAuthorityModel> GetFirst()
        {
            var entity = await _Context.PolicyRequirementLocalAuthorities.FirstOrDefaultAsync();

            if (entity != null)
            {
                var model = MapToModel(entity);

                return model;
            }

            return default(PolicyRequirementLocalAuthorityModel);
        }

        public async Task<PolicyRequirementLocalAuthorityModel> GetOneModelAsync(Guid id)
        {
            var entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                var model = MapToModel(entity);

                return model;
            }

            return default(PolicyRequirementLocalAuthorityModel);
        }

        public async Task<int> Update(PolicyRequirementLocalAuthorityModel model)
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
                _Context.PolicyRequirementLocalAuthorities.DeleteObject(entity);

                result = 1;
            }

            return result;
        }
    }
}
