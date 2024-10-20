using DALI.PolicyRequirements.DataEntityModels;
using DALI.PolicyRequirements.DomainModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.Repositories
{
    public class PolicyRequirementChildSubjectRepository : Repository
    {
        public PolicyRequirementChildSubjectRepository(IPolicyRequirementDataEntityModelContext context) : base(context)
        {
        }

        private PolicyRequirementChildSubject MapToEntity(PolicyRequirementChildSubjectModel model)
        {
            var entity = new PolicyRequirementChildSubject();

            entity.ID = model.Id;
            entity.Description = model.Description;
            entity.IsActive = model.Active ? 1 : 0;
            entity.VersionId = model.VersionId;
            entity.FetchByDefault = model.FetchByDefault ? 1 : 0;
            entity.IsTownSpecific = model.IsTownSpecific;

            return entity;
        }

        private PolicyRequirementChildSubjectModel MapToModel(PolicyRequirementChildSubject entity)
        {
            var model = new PolicyRequirementChildSubjectModel();

            // Fill model properties
            model.Id = entity.ID;
            model.Active = (entity.IsActive == 1);
            model.Description = entity.Description;
            model.VersionId = entity.VersionId;
            model.FetchByDefault = entity.FetchByDefault == 1;
            model.IsTownSpecific = entity.IsTownSpecific == true;

            return model;
        }

        public async Task<List<PolicyRequirementChildSubjectModel>> GetAll(int versionId)
        {
            var entities = await _Context.PolicyRequirementChildSubjects.Where(e => e.VersionId == versionId && e.IsActive == 1).OrderBy(e => e.Description).ToArrayAsync();

            var models = entities.Select(e => MapToModel(e)).ToList();

            return models;
        }

        public async Task<List<PolicyRequirementChildSubjectModel>> GetForEditors(int versionId)
        {
            var entities = await _Context.PolicyRequirementChildSubjects.AsNoTracking().Where(e => (e.VersionId == versionId || e.VersionId == -1) && e.IsActive == 1).OrderBy(e => e.Description).ToArrayAsync();

            var models = entities.Select(e => MapToModel(e)).ToList();

            return models;
        }

        public async Task<PolicyRequirementChildSubjectModel> GetOneModelAsync(int id, int versionId)
        {
            var entity = await _Context.PolicyRequirementChildSubjects.AsNoTracking().Where(e => e.ID == id && e.VersionId == versionId).SingleOrDefaultAsync();

            if (entity != null)
            {
                var model = MapToModel(entity);

                return model;
            }

            return default(PolicyRequirementChildSubjectModel);
        }

        public async Task<List<PolicyRequirementChildSubjectModel>> GetAll(int[] chapters, int[] levels, Guid[] locations, int[] areas, int[] subjects, int versionId, IList<PolicyRequirementModel> models)
        {
            List<PolicyRequirementModel> items = new List<PolicyRequirementModel>();

            if (chapters != null && chapters.Length > 0)
            {
                var chapterItems = models.Where(m => chapters.Contains(m.Chapter.Id));
                items.AddRange(chapterItems);
            }
            else
            {
                items.AddRange(models);
            }

            if (levels != null && levels.Length > 0)
            {
                var levelItems = items.Where(m => levels.Contains(m.Level.Id));
                items = new List<PolicyRequirementModel>(levelItems);
            }

            if (locations != null && locations.Length > 0)
            {
                var locationModels = items.Where(m => locations.Contains(m.Location.Id));
                items = new List<PolicyRequirementModel>(locationModels);
            }

            if (areas != null && areas.Length > 0)
            {
                var areaModels = items.Where(m => areas.Contains(m.Area.Id));
                items = new List<PolicyRequirementModel>(areaModels);
            }

            if (subjects != null && subjects.Length > 0)
            {
                var subjectModels = items.Where(m => subjects.Contains(m.Subject.Id));
                items = new List<PolicyRequirementModel>(subjectModels);
            }

            if (items.Count > 0)
            {
                int[] ids = items.Select(m => m.ChildSubject.Id).Distinct().ToArray();

                var childSubjects = await GetAll(versionId);

                return childSubjects.Where(m => ids.Contains(m.Id)).ToList();
            }

            return new List<PolicyRequirementChildSubjectModel>();
        }
    }
}
