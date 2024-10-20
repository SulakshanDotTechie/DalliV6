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
    public class DesignBriefSubjectRepository : Repository
    {
        public DesignBriefSubjectRepository(IDesignBriefDataEntityModelContext context) : base(context)
        {

        }

        private DesignBriefPolicyRequirementSubject MapToEntity(DesignBriefSubjectModel model)
        {
            DesignBriefPolicyRequirementSubject entity = new DesignBriefPolicyRequirementSubject
            {
                ID = model.Id,
                VersionId = model.VersionId,
                Description = model.Description,
                DesignBriefId = model.DesignBriefId
            };

            return entity;
        }

        private DesignBriefSubjectModel MapToModel(DesignBriefPolicyRequirementSubject entity)
        {
            DesignBriefSubjectModel model = new DesignBriefSubjectModel
            {
                Id = entity.ID,
                VersionId = entity.VersionId,
                Description = entity.Description,
                DesignBriefId = entity.DesignBriefId
            };

            return model;
        }

        private async Task<DesignBriefPolicyRequirementSubject> GetOneEntityAsync(int designBriefId, int id)
        {
            DesignBriefPolicyRequirementSubject entity = await _Context.DesignBriefPolicyRequirementSubjects.SingleOrDefaultAsync(e => e.ID == id && e.DesignBriefId == designBriefId);

            return entity;
        }

        public async Task<DesignBriefSubjectModel[]> GetBy(int designBriefId)
        {
            DesignBriefPolicyRequirementSubject[] entities = await _Context.DesignBriefPolicyRequirementSubjects.Where(e => e.DesignBriefId == designBriefId).ToArrayAsync();

            DesignBriefSubjectModel[] models = entities.Select(e => MapToModel(e)).OrderBy(m => m.Description).ToArray();

            return models;
        }

        public async Task<int> Add(DesignBriefSubjectModel model)
        {
            int result = 0;

            using (DesignBriefDataEntityModels context = NewContext())
            {
                DesignBriefPolicyRequirementSubject entity = MapToEntity(model);

                context.DesignBriefPolicyRequirementSubjects.AddObject(entity);

                await context.SaveChangesAsync();

                model.Id = entity.ID;

                result = 1;
            }

            return result;
        }

        public async Task<int> Update(DesignBriefSubjectModel model)
        {
            int result = 0;

            var entity = await GetOneEntityAsync(model.DesignBriefId, model.Id);

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

            DesignBriefPolicyRequirementSubject entity = await GetOneEntityAsync(designBriefId, id);

            if (entity != null)
            {
                _Context.DesignBriefPolicyRequirementSubjects.DeleteObject(entity);

                result = 1;
            }

            return result;
        }
    }
}
