using DALI.DesignBrief.DataEntityModels;
using DALI.DesignBrief.DomainModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.DesignBrief.Repositories
{
    public class DesignBriefChapterRepository : Repository
    {
        public DesignBriefChapterRepository(IDesignBriefDataEntityModelContext context) : base(context)
        {

        }

        private DesignBriefPolicyRequirementChapter MapToEntity(DesignBriefChapterModel model)
        {
            DesignBriefPolicyRequirementChapter entity = new DesignBriefPolicyRequirementChapter();

            entity.ID = model.Id;
            entity.VersionId = model.VersionId;
            entity.ChapterNumber = model.ChapterNumber;
            entity.Description = model.Description;
            entity.DesignBriefId = model.DesignBriefId;

            return entity;
        }

        private DesignBriefChapterModel MapToModel(DesignBriefPolicyRequirementChapter entity)
        {
            DesignBriefChapterModel model = new DesignBriefChapterModel();

            model.Id = entity.ID;
            model.VersionId = entity.VersionId;
            model.ChapterNumber = entity.ChapterNumber;
            model.Description = entity.Description;
            model.DesignBriefId = entity.DesignBriefId;

            var topic = _Context.DesignBriefTopics.SingleOrDefault(e => e.LiorId == model.Id && e.DesignBriefId == model.DesignBriefId);
            if (topic != null)
            {
                model.Owner = topic.Owner;
                model.OwnerEmailAddress = topic.OwnerEmailAddress;
            }

            return model;
        }

        private async Task<DesignBriefPolicyRequirementChapter> GetOneEntityAsync(int designBriefId, int id)
        {
            DesignBriefPolicyRequirementChapter entity = await _Context.DesignBriefPolicyRequirementChapters.SingleOrDefaultAsync(e => e.ID == id && e.DesignBriefId == designBriefId);

            return entity;
        }

        public async Task<DesignBriefChapterModel[]> GetBy(int designBriefId)
        {
            DesignBriefPolicyRequirementChapter[] entities = await _Context.DesignBriefPolicyRequirementChapters.Where(e => e.DesignBriefId == designBriefId).ToArrayAsync();

            DesignBriefChapterModel[] models = entities.Select(e => MapToModel(e)).OrderBy(e => e.FullChapterDescription).ToArray();

            return models;
        }

        public async Task<int> Add(DesignBriefChapterModel model)
        {
            int result = 0;

            using (DesignBriefDataEntityModels context = NewContext())
            {
                DesignBriefPolicyRequirementChapter entity = MapToEntity(model);

                context.DesignBriefPolicyRequirementChapters.AddObject(entity);
                await context.SaveChangesAsync();

                model.Id = entity.ID;

                result = 1;
            }

            return result;
        }

        public async Task<int> Update(DesignBriefChapterModel model)
        {
            int result = 0;

            DesignBriefPolicyRequirementChapter entity = await GetOneEntityAsync(model.DesignBriefId, model.Id);

            if (entity != null)
            {
                entity.Description = model.Description;

                result = 1;
            }

            return result;
        }

        public async Task<int> Delete(int designBriefId, int id)
        {
            int result = 0;

            DesignBriefPolicyRequirementChapter entity = await GetOneEntityAsync(designBriefId, id);

            if (entity != null)
            {
                _Context.DesignBriefPolicyRequirementChapters.DeleteObject(entity);

                result = 1;
            }

            return result;
        }
    }
}
