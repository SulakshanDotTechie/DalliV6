using DALI.PolicyRequirements.DataEntityModels;
using DALI.PolicyRequirements.DomainModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.Repositories
{
    public class PolicyRequirementKeywordRepository : Repository
    {
        public PolicyRequirementKeywordRepository(IPolicyRequirementDataEntityModelContext context) : base(context)
        {
        }

        private PolicyRequirementKeyword MapToEntity(PolicyRequirementKeywordModel model)
        {
            var entity = new PolicyRequirementKeyword();

            entity.Id = model.Id;
            entity.Description = model.Description;
            entity.IsActive = model.Active ? 1 : 0;
            entity.VersionId = model.VersionId;

            return entity;
        }

        private PolicyRequirementKeywordModel MapToModel(PolicyRequirementKeyword entity)
        {
            var model = new PolicyRequirementKeywordModel();

            // Fill model properties
            model.Id = entity.Id;
            model.Active = (entity.IsActive == 1);
            model.Description = entity.Description;

            return model;
        }

        private async Task<PolicyRequirementKeyword> GetOneEntityAsync(int id, int versionId)
        {
            var entity = await _Context.PolicyRequirementKeywords.SingleOrDefaultAsync(e => e.Id == id && (e.VersionId == versionId || e.VersionId == -1));

            return entity;
        }

        public async Task<int> Add(PolicyRequirementKeywordModel model)
        {
            int result = 0;


            using (var context = NewContext())
            {
                var entity = MapToEntity(model);

                int nextId = await GetNextIdForEntity(entity);

                entity.Id = nextId;

                context.PolicyRequirementKeywords.AddObject(entity);
                await context.SaveChangesAsync();

                model.Id = nextId;
                result = 1;
            }

            return result;
        }

        public int AddWithInitializedId(PolicyRequirementKeywordModel model)
        {
            int result = 0;

            var entity = MapToEntity(model);

            _Context.PolicyRequirementKeywords.AddObject(entity);

            result = 1;

            return result;
        }

        public async Task<PolicyRequirementKeywordModel> GetByDescription(string description, int versionId)
        {
            PolicyRequirementKeyword entity = await _Context.PolicyRequirementKeywords.SingleOrDefaultAsync(e => string.Compare(e.Description, description, true) == 0 && (e.VersionId == versionId || e.VersionId == -1));

            if (entity != null)
            {
                var model = MapToModel(entity);
                return model;
            }

            return default(PolicyRequirementKeywordModel);
        }

        public async Task<List<PolicyRequirementKeywordModel>> GetAll(int versionId)
        {
            var keywords = await _Context.PolicyRequirementKeywords.Where(e => e.IsActive == 1 && e.VersionId == versionId).Select(e => e.Description).Distinct().ToArrayAsync();
            var subjects = await _Context.PolicyRequirementSubjects.Where(e => e.IsActive == 1 && e.VersionId == versionId).Select(e => e.Description).Distinct().ToArrayAsync();
            var attachments = await _Context.PolicyRequirementAssignedAttachments.Where(e => e.IsActive == 1 && e.VersionId == versionId && e.PolicyRequirementAttachment != null).Select(e => e.PolicyRequirementAttachment.Description).Distinct().ToArrayAsync();
            var sourceReferences = await _Context.PolicyRequirementAssignedSources.Where(e => e.IsActive == 1 && e.VersionId == versionId && e.PolicyRequirementSource != null).Select(e => e.PolicyRequirementSource.Description).Distinct().ToArrayAsync();

            List<string> keywordsList = new List<string>();
            keywordsList.AddRange(keywords);
            keywordsList.AddRange(subjects);
            keywordsList.AddRange(attachments);
            keywordsList.AddRange(sourceReferences);
            keywordsList.Sort();
            int id = 1;

            var models = keywordsList.Distinct().Select(e => new PolicyRequirementKeywordModel()
            {
                Id = id++,
                Active = true,
                Description = e
            }).ToList();

            return models;
        }

        public async Task<PolicyRequirementKeywordModel> GetOneModelAsync(int id, int versionId)
        {
            var entity = await GetOneEntityAsync(id, versionId);

            if (entity != null)
            {
                var model = MapToModel(entity);
                return model;
            }

            return default(PolicyRequirementKeywordModel);
        }

        public async Task<int> Update(PolicyRequirementKeywordModel model)
        {
            int result = 0;

            var entity = await GetOneEntityAsync(model.Id, model.VersionId);

            if (entity != null)
            {
                entity.Description = model.Description;

                result = 1;
            }

            return result;
        }

        public async Task<int> Delete(int id, int versionId)
        {
            int result = 0;

            var entity = await GetOneEntityAsync(id, versionId);

            if (entity != null)
            {
                _Context.PolicyRequirementKeywords.DeleteObject(entity);

                result = 1;
            }

            return result;
        }
    }
}
