using DALI.DesignBrief.DataEntityModels;
using DALI.DesignBrief.DomainModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.DesignBrief.Repositories
{
    public class DesignBriefSeverityRepository : Repository
    {
        public DesignBriefSeverityRepository(IDesignBriefDataEntityModelContext context) : base(context)
        {

        }

        private DesignBriefPolicyRequirementStrictness MapToEntity(DesignBriefSeverityModel model)
        {
            DesignBriefPolicyRequirementStrictness entity = new DesignBriefPolicyRequirementStrictness
            {
                ID = model.Id,
                ShortName = model.ShortName,
                Description = model.Description,
                HelpText = model.Clarification
            };

            return entity;
        }

        private DesignBriefSeverityModel MapToModel(DesignBriefPolicyRequirementStrictness entity)
        {
            DesignBriefSeverityModel model = new DesignBriefSeverityModel
            {

                // Fill model properties
                Id = entity.ID,
                ShortName = entity.ShortName,
                Description = entity.Description,
                Clarification = entity.HelpText
            };

            return model;
        }

        private async Task<DesignBriefPolicyRequirementStrictness> GetOneEntityAsync(int id)
        {
            DesignBriefPolicyRequirementStrictness entity = await _Context.DesignBriefPolicyRequirementStrictnesses.SingleOrDefaultAsync(e => e.ID == id);

            return (DesignBriefPolicyRequirementStrictness)entity;
        }

        public async Task<int> Add(DesignBriefSeverityModel model)
        {
            int result = 0;

            await Task.Run(() => {
                var entity = MapToEntity(model);

                int nextId = GetNextIdForEntity<DesignBriefPolicyRequirementStrictness>(entity);

                entity.ID = nextId;

                _Context.DesignBriefPolicyRequirementStrictnesses.AddObject(entity);

                model.Id = nextId;

                result = 1;
            });

            return result;
        }

        public async Task<DesignBriefSeverityModel[]> GetAll()
        {
            DesignBriefPolicyRequirementStrictness[] entities = await _Context.DesignBriefPolicyRequirementStrictnesses.OrderBy(e => e.Description).ToArrayAsync();

            DesignBriefSeverityModel[] models = entities.Select(e => MapToModel(e)).ToArray();

            return models;
        }

        public async Task<DesignBriefSeverityModel> GetOneModelAsync(int id)
        {
            DesignBriefPolicyRequirementStrictness entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                DesignBriefSeverityModel model = MapToModel(entity);
                return model;
            }

            return default(DesignBriefSeverityModel);
        }

        public async Task<int> Update(DesignBriefSeverityModel model)
        {
            int result = 0;

            DesignBriefPolicyRequirementStrictness entity = await GetOneEntityAsync(model.Id);

            if (entity != null)
            {
                entity.Description = model.Description;
                entity.HelpText = model.Clarification;

                result = 1;
            }

            return result;
        }

        public async Task<int> Delete(int id)
        {
            int result = 0;

            DesignBriefPolicyRequirementStrictness entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                _Context.DesignBriefPolicyRequirementStrictnesses.DeleteObject(entity);

                result = 1;
            }

            return result;
        }
    }
}
