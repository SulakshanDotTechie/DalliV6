using DALI.PolicyRequirements.DataEntityModels;
using DALI.PolicyRequirements.DomainModels;

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.Repositories
{
    public class PolicyRequirementPublicationRepository : Repository
    {
        private readonly string _DefaultPublicationScheduleStartTime = "DefaultPublicationScheduleStartTime";

        public PolicyRequirementPublicationRepository(IPolicyRequirementDataEntityModelContext context) : base(context)
        {
        }

        public async Task<int> GetCurrentVersion()
        {
            int version = -1000;

            PolicyRequirementPublishingControl[] versions = await _Context.PolicyRequirementPublishingControls.Where(e => e.IsPublished.HasValue && e.IsPublished.Value == true).ToArrayAsync();

            if (versions != null && versions.Length > 0)
                version = versions.Max(e => e.Id);

            return version;
        }

        /// <summary>
        /// Returs the latest release number else returns -1 of not available
        /// </summary>
        /// <returns></returns>
        public async Task<PolicyRequirementPublicationModel> GetLatest()
        {
            PolicyRequirementPublishingControl entity = await _Context.PolicyRequirementPublishingControls.Where(e => e.IsPublished.HasValue && e.IsPublished.Value).OrderByDescending(e => e.Id).FirstOrDefaultAsync();

            if (entity != null)
            {
                return MapToModel(entity);
            }

            return null;
        }

        public async Task<PolicyRequirementPublicationModel> GetScheduled()
        {
            PolicyRequirementPublishingControl entity = await _Context.PolicyRequirementPublishingControls.Where(e => e.IsPublished == null || e.IsPublished == false).OrderByDescending(e => e.Id).FirstOrDefaultAsync();

            if (entity != null)
            {
                return MapToModel(entity);
            }

            return null;
        }

        private PolicyRequirementPublishingControl MapToEntity(PolicyRequirementPublicationModel model)
        {
            PolicyRequirementPublishingControl entity = new PolicyRequirementPublishingControl();

            entity.Id = model.Id;
            entity.CreatedDate = DateTime.Now;
            entity.StartTime = model.StartTime;
            entity.EndTime = model.EndTime;
            entity.IsPublished = model.IsPublished;
            entity.PublishedBy = model.PublishedBy;
            entity.Description = model.Description;
            entity.PublishInfo = model.Info;

            return entity;
        }

        private PolicyRequirementPublicationModel MapToModel(PolicyRequirementPublishingControl entity)
        {
            var model = new PolicyRequirementPublicationModel();

            model.Id = entity.Id;
            model.CreatedDate = entity.CreatedDate;
            model.StartTime = (DateTime)entity.StartTime;
            model.EndTime = entity.EndTime;
            model.IsPublished = entity.IsPublished;
            model.PublishedBy = entity.PublishedBy;
            model.Description = entity.Description;
            model.Info = entity.PublishInfo;
            model.VersionId = model.Id;

            return model;
        }

        private async Task<PolicyRequirementPublishingControl> GetOneEntityAsync(int id)
        {
            PolicyRequirementPublishingControl entity = await _Context.PolicyRequirementPublishingControls.Where(e => e.Id == id).SingleOrDefaultAsync();

            return entity;
        }

        public async Task<PolicyRequirementPublicationModel> GetOneModelAsync(int id)
        {
            PolicyRequirementPublishingControl entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                PolicyRequirementPublicationModel model = MapToModel(entity);
                return model;
            }

            return default(PolicyRequirementPublicationModel);
        }

        public async Task<List<PolicyRequirementPublicationModel>> GetAll()
        {
            PolicyRequirementPublishingControl[] entities = await _Context.PolicyRequirementPublishingControls.ToArrayAsync();
            List<PolicyRequirementPublicationModel> models = new List<PolicyRequirementPublicationModel>();
            foreach (var entity in entities)
            {
                PolicyRequirementPublicationModel model = MapToModel(entity);
                models.Add(model);
            }

            if (models.Count > 0)
            {
                return models.OrderByDescending(m => m.Id).ToList();
            }

            return models;
        }

        public async Task<int> Add(PolicyRequirementPublicationModel model)
        {
            int result = 0;

            int id = (await GetAll()).Count;

            PolicyRequirementPublishingControl entity = new PolicyRequirementPublishingControl();

            entity.Id = ++id;
            entity.CreatedDate = DateTime.Now;
            entity.StartTime = model.StartTime;
            entity.EndTime = model.EndTime;
            entity.IsPublished = model.IsPublished;
            entity.PublishedBy = model.PublishedBy;
            entity.Description = model.Description;

            model.Id = entity.Id;

            _Context.PolicyRequirementPublishingControls.AddObject(entity);

            result = 1;

            return result;
        }

        public async Task<int> Update(PolicyRequirementPublicationModel model)
        {
            var result = 0;

            var entity = await GetOneEntityAsync(model.Id);

            if (null != entity && entity.IsPublished.HasValue && !entity.IsPublished.Value)
            {
                entity.StartTime = model.StartTime;
                entity.Description = model.Description;
                result = 1;
            }

            return result;
        }

        public async Task<int> Delete(int id)
        {
            var result = 0;

            var entity = await GetOneEntityAsync(id);

            if (null != entity)
            {
                _Context.PolicyRequirementPublishingControls.DeleteObject(entity);
                result = 1;
            }

            return result;
        }

        public async Task<int> GetDefaultPublicationScheduleStartTime()
        {
            var entity = await _Context.APP_SETTINGS.SingleOrDefaultAsync(e => string.Compare(e.SettingsName, _DefaultPublicationScheduleStartTime, true) == 0);

            return entity == null ? 24 : Convert.ToInt32(entity.SettingsValues);
        }
    }
}
