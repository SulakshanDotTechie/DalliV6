using DALI.DesignBrief.DataEntityModels;
using DALI.DesignBrief.DomainModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.DesignBrief.Repositories
{
    public class DesignBriefTypeRepository : Repository
    {
        public DesignBriefTypeRepository(IDesignBriefDataEntityModelContext context) : base(context)
        {

        }

        private DesignBriefType MapToEntity(DesignBriefTypeModel model)
        {
            DesignBriefType entity = new DesignBriefType
            {
                Id = model.Id,
                Name = model.Description
            };

            return entity;
        }

        private DesignBriefTypeModel MapToModel(DesignBriefType entity)
        {
            DesignBriefTypeModel model = new DesignBriefTypeModel
            {

                // Fill model properties
                Id = entity.Id,
                Description = entity.Name
            };

            return model;
        }


        private async Task<DesignBriefType> GetOneEntityAsync(int id)
        {
            DesignBriefType entity = await _Context.DesignBriefTypes.SingleOrDefaultAsync(e => e.Id == id);

            return entity;
        }

        public async Task<DesignBriefTypeModel[]> GetAll()
        {
            DesignBriefType[] entities = await _Context.DesignBriefTypes.ToArrayAsync();

            DesignBriefTypeModel[] models = entities.OrderBy(m => m.OrderIndex).Select(e => MapToModel(e)).ToArray();

            return models;
        }

        public async Task<DesignBriefTypeModel> GetOneModelAsync(int id)
        {
            DesignBriefType entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                DesignBriefTypeModel model = MapToModel(entity);

                return model;
            }

            return default(DesignBriefTypeModel);
        }

        public async Task<int> Update(DesignBriefTypeModel model)
        {
            int result = 0;

            DesignBriefType entity = await GetOneEntityAsync(model.Id);

            if (entity != null)
            {
                entity.Name = model.Description;

                result = 1;
            }

            return result;
        }

        public async Task<int> Delete(int id)
        {
            int result = 0;

            DesignBriefType entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                int count = _Context.DesignBriefTypes.Where(e => e.Id == id).Count();

                if (count == 0)
                {

                    _Context.DesignBriefTypes.DeleteObject(entity);

                    result = 1;
                }
                else
                {
                    result = 2;
                }
            }

            return result;
        }

        public async Task<int> Add(DesignBriefTypeModel model)
        {
            int result = 0;

            DesignBriefType entity = MapToEntity(model);

            _Context.DesignBriefTypes.AddObject(entity);

            await Save();

            model.Id = entity.Id;

            result = 1;

            return result;
        }
    }
}
