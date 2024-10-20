using DALI.DesignBrief.DataEntityModels;
using DALI.DesignBrief.DomainModels;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.DesignBrief.Repositories
{
    public class DesignBriefLocationRepository : Repository
    {
        public DesignBriefLocationRepository(IDesignBriefDataEntityModelContext context) : base(context)
        {
        }

        private DesignBriefPolicyRequirementLocation MapToEntity(DesignBriefLocationModel model)
        {
            DesignBriefPolicyRequirementLocation entity = new DesignBriefPolicyRequirementLocation
            {
                Id = model.Id,
                Description = model.Description,
                FetchByDefault = model.FetchByDefault
            };

            return entity;
        }

        private DesignBriefLocationModel MapToModel(DesignBriefPolicyRequirementLocation entity)
        {
            DesignBriefLocationModel model = new DesignBriefLocationModel
            {

                // Fill model properties
                Id = entity.Id,
                Description = entity.Description,
                FetchByDefault = entity.FetchByDefault ?? (default(bool))
            };

            return model;
        }

        private async Task<DesignBriefPolicyRequirementLocation> GetOneEntityAsync(Guid id)
        {
            DesignBriefPolicyRequirementLocation entity = await _Context.DesignBriefPolicyRequirementLocations.SingleOrDefaultAsync(e => e.Id == id);

            return entity;
        }

        public async Task<Guid[]> GetFetchByDefaultIds()
        {
            Guid[] ids = await _Context.DesignBriefPolicyRequirementLocations.Where(e => e.FetchByDefault == true).Select(e => e.Id).Distinct().ToArrayAsync();

            return ids;
        }

        public async Task<int> Add(DesignBriefLocationModel model)
        {
            int result = 0;

            DesignBriefPolicyRequirementLocation entity = await GetOneEntityAsync(model.Id);

            if (entity == null)
            {
                entity = MapToEntity(model);

                _Context.DesignBriefPolicyRequirementLocations.AddObject(entity);

                model.Id = entity.Id;

                result = 1;
            }

            return result;
        }

        public async Task<DesignBriefLocationModel[]> GetAll()
        {
            DesignBriefPolicyRequirementLocation[] entities = await _Context.DesignBriefPolicyRequirementLocations.ToArrayAsync();

            DesignBriefLocationModel[] models = entities.Select(e => MapToModel(e)).OrderBy(e => e.Description).ToArray();

            return models;
        }

        public async Task<DesignBriefLocationModel> GetOneModelAsync(Guid id)
        {
            DesignBriefPolicyRequirementLocation entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                var model = MapToModel(entity);

                return model;
            }

            return default(DesignBriefLocationModel);
        }

        public async Task<int> Update(DesignBriefLocationModel model)
        {
            int result = 0;

            DesignBriefPolicyRequirementLocation entity = await GetOneEntityAsync(model.Id);

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

            DesignBriefPolicyRequirementLocation entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                _Context.DesignBriefPolicyRequirementLocations.DeleteObject(entity);

                result = 1;
            }

            return result;
        }
    }
}
