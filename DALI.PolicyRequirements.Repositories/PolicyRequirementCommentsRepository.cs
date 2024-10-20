using DALI.PolicyRequirements.DataEntityModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.Repositories
{
    public class PolicyRequirementCommentsRepository : Repository
    {
        public PolicyRequirementCommentsRepository(IPolicyRequirementDataEntityModelContext context) : base(context)
        {
        }

        public async Task<string[]> GetAll(int policyRequirementId)
        {
            //&& !e.ActionId.HasValue

            var entities = await _Context.PolicyRequirementPublishingAdditionalInfoes.Where(e => e.PolicyRequirementId == policyRequirementId).ToArrayAsync();

            var orderEntities = entities.OrderByDescending(e => e.Id);

            if (orderEntities.Count() > 0)
                return new string[] { orderEntities.First().Description, orderEntities.First().CreatedBy };

            return new string[] { };
        }

        public async Task<string[]> GetAll(int policyRequirementId, string userName)
        {
            var entities = await _Context.PolicyRequirementPublishingAdditionalInfoes.Where(e => e.PolicyRequirementId == policyRequirementId && e.CreatedBy == userName).ToArrayAsync();

            var orderEntities = entities.OrderByDescending(e => e.Id);

            if (orderEntities.Count() > 0)
                return new string[] { entities.First().Description, entities.First().CreatedBy };

            return new string[] { };
        }

        public async Task<int> Add(string info, int policyRequirementId, string description, string userName, int? actionId, int version)
        {
            var result = 0;

            var entity = new PolicyRequirementPublishingAdditionalInfo();

            entity.Id = await GetNextIdForEntity<PolicyRequirementPublishingAdditionalInfo>(entity);

            entity.CreatedBy = userName;
            entity.CreatedDate = DateTime.Now;
            entity.ActionId = actionId;
            entity.PolicyRequirementId = policyRequirementId;
            entity.PolicyRequirementDescription = description;
            entity.Description = info;
            entity.VersionId = version;

            _Context.PolicyRequirementPublishingAdditionalInfoes.AddObject(entity);

            result = 1;

            return result;
        }
    }
}
