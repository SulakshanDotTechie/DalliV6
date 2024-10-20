using DALI.DesignBrief.DataEntityModels;
using DALI.DesignBrief.DomainModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.DesignBrief.Repositories
{
    public class DesignBriefChildSubjectRepository : Repository
    {
        public DesignBriefChildSubjectRepository(IDesignBriefDataEntityModelContext context) : base(context)
        {
        }

        private DesignBriefPolicyRequirementChildSubject MapToEntity(DesignBriefChildSubjectModel model)
        {
            var entity = new DesignBriefPolicyRequirementChildSubject();

            entity.ID = model.Id;
            entity.Description = model.Description;

            return entity;
        }

        private DesignBriefChildSubjectModel MapToModel(DesignBriefPolicyRequirementChildSubject entity)
        {
            var model = new DesignBriefChildSubjectModel();

            // Fill model properties
            model.Id = entity.ID;
            model.Description = entity.Description;

            return model;
        }

        public async Task<DesignBriefChildSubjectModel[]> GetAll(int designBriefId, int versionId)
        {
            DesignBriefPolicyRequirementChildSubject[] entities = await _Context.DesignBriefPolicyRequirementChildSubjects.Where(e => e.DesignBriefId == designBriefId && e.VersionId == versionId).ToArrayAsync();

            DesignBriefChildSubjectModel[] models = entities.Select(e => MapToModel(e)).OrderBy(e => e.Description).ToArray();

            return models;
        }


        public async Task<DesignBriefChildSubjectModel[]> GetForEditors(int designBriefId, int versionId)
        {
            DesignBriefPolicyRequirementChildSubject[] entities = await _Context.DesignBriefPolicyRequirementChildSubjects.Where(e => e.DesignBriefId == designBriefId && (e.VersionId == versionId || e.VersionId == -1)).ToArrayAsync();

            DesignBriefChildSubjectModel[] models = entities.Select(e => MapToModel(e)).OrderBy(e => e.Description).ToArray();

            return models;
        }

        public async Task<DesignBriefChildSubjectModel> GetOneModelAsync(int id, int versionId)
        {
            DesignBriefPolicyRequirementChildSubject entity = await _Context.DesignBriefPolicyRequirementChildSubjects.Where(e => e.ID == id && e.VersionId == versionId).SingleOrDefaultAsync();

            if (entity != null)
            {
                DesignBriefChildSubjectModel model = MapToModel(entity);
                return model;
            }

            return default(DesignBriefChildSubjectModel);
        }
    }
}
