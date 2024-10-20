using DALI.PolicyRequirements.DataEntityModels;
using DALI.PolicyRequirements.DomainModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.Repositories
{
    public class PolicyRequirementPublicationQueueRepository : Repository
    {
        public PolicyRequirementPublicationQueueRepository(IPolicyRequirementDataEntityModelContext context) : base(context)
        {
        }

        private PolicyRequirementPublishQueue MapToEntity(PolicyRequirementPublicationQueueModel model)
        {
            var entity = new PolicyRequirementPublishQueue()
            {
                ID = model.Id,
                IsActive = model.Active,
                CanPublish = model.CanPublish,
                CreatedDate = model.CreatedDate
            };
            return entity;
        }

        private PolicyRequirementPublicationQueueModel MapToModel(PolicyRequirementPublishQueue entity)
        {
            var model = new PolicyRequirementPublicationQueueModel();

            model.Id = entity.ID;
            model.Active = (entity.IsActive.HasValue && entity.IsActive.Value == false);
            model.CanPublish = entity.CanPublish;
            model.CreatedDate = entity.CreatedDate;

            return model;
        }

        public async Task<List<PolicyRequirementPublicationQueueModel>> GetAll()
        {
            var models = new List<PolicyRequirementPublicationQueueModel>();

            var entities = await _Context.PolicyRequirementPublishQueues.Where(e => e.VersionId == null).ToArrayAsync();

            foreach (var entity in entities)
            {
                var model = MapToModel(entity);
                models.Add(model);
            }

            return models;
        }


        protected async Task<PolicyRequirementPublishQueue> GetOneEntityAsync(int id)
        {
            var entity = await _Context.PolicyRequirementPublishQueues.SingleOrDefaultAsync(e => e.ID == id && e.VersionId == null);

            return entity;
        }


        public async Task<PolicyRequirementPublicationQueueModel> GetOneModelAsync(int id)
        {
            var entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                var model = MapToModel(entity);
                return model;
            }

            return default(PolicyRequirementPublicationQueueModel);
        }

        public async Task<int> Update(PolicyRequirementPublicationQueueModel model)
        {
            var result = 0;

            var entity = await _Context.PolicyRequirementPublishQueues.SingleOrDefaultAsync(e => e.ID == model.Id && e.VersionId == null);

            if (entity != null)
            {
                entity.CanPublish = model.CanPublish;

                result = 1;
            }

            return result;
        }

        public async Task<int> Add(PolicyRequirementPublicationQueueModel model)
        {
            int result = 0;

            await Task.Run(() =>
            {
                var entity = MapToEntity(model);

                _Context.PolicyRequirementPublishQueues.AddObject(entity);

                result = 1;
            });

            return result;
        }

        public async Task<int> Delete(int id)
        {
            int result = 0;

            var entity = await _Context.PolicyRequirementPublishQueues.Where(e => e.ID == id && e.VersionId == null).SingleOrDefaultAsync();

            if (entity != null)
            {
                _Context.PolicyRequirementPublishQueues.DeleteObject(entity);
                result = 1;
            }

            return result;
        }

        public async Task<bool> ModificationsToBePublished()
        {
            var count = await _Context.PolicyRequirementPublishQueues.Where(e => e.CanPublish == true && e.VersionId == null).CountAsync();

            return count > 0;
        }
    }
}
