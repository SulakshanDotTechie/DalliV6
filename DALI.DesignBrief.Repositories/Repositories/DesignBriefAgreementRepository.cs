using DALI.DesignBrief.DataEntityModels;
using DALI.DesignBrief.DomainModels;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.DesignBrief.Repositories
{
    public class DesignBriefAgreementRepository : Repository
    {
        public DesignBriefAgreementRepository(IDesignBriefDataEntityModelContext context) : base(context)
        {

        }

        private DesignBriefAgreement MapToEntity(DesignBriefAgreementModel model)
        {
            DesignBriefAgreement entity = new DesignBriefAgreement();

            entity.Id = model.Id;
            entity.Name = model.Description;

            return entity;
        }


        private IQueryable<DesignBriefAgreement> GetEntities()
        {
            var entities = _Context.DesignBriefAgreements;

            return entities;
        }

        private async Task<DesignBriefAgreement> GetOneEntityAsync(int id)
        {
            DesignBriefAgreement[] entities = await GetEntities().ToArrayAsync();

            DesignBriefAgreement entity = entities.SingleOrDefault(e => e.Id == id);

            return entity;
        }

        public async Task<DesignBriefAgreementModel[]> GetAll()
        {
            DesignBriefAgreement[] entities = await GetEntities().ToArrayAsync();

            DesignBriefAgreementModel[] models = entities.OrderBy(e => e.OrderIndex).Select(e => new DesignBriefAgreementModel()
            {
                Id = e.Id,
                Description = e.Name
            }).ToArray();

            return models;
        }

        public async Task<DesignBriefAgreementModel> GetOneModelAsync(int id)
        {
            DesignBriefAgreement entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                DesignBriefAgreementModel model = new DesignBriefAgreementModel()
                {
                    Id = entity.Id,
                    Description = entity.Name
                };

                return model;
            }

            return default(DesignBriefAgreementModel);
        }

        public async Task<int> Update(DesignBriefAgreementModel model)
        {
            int result = 0;

            DesignBriefAgreement entity = await GetOneEntityAsync(model.Id);

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

            DesignBriefAgreement entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                var count = _Context.DesignBriefs.Where(e => e.AgreementId == id).Count();

                if (count == 0)
                {

                    _Context.DesignBriefAgreements.DeleteObject(entity);

                    result = 1;
                }
                else
                {
                    result = 2;
                }
            }

            return result;
        }

        public async Task<int> Add(DesignBriefAgreementModel model)
        {
            int result = 0;

            using (DesignBriefDataEntityModels context = NewContext())
            {
                var entity = MapToEntity(model);

                context.DesignBriefAgreements.AddObject(entity);
                await context.SaveChangesAsync();

                model.Id = entity.Id;

                result = 1;
            }

            return result;
        }
    }
}
