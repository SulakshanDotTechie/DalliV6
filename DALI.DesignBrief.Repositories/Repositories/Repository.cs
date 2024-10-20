using DALI.DesignBrief.DataEntityModels;
using DALI.DesignBrief.DomainModels;
using DALI.PolicyRequirements.DataEntityModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using IEntity = DALI.DesignBrief.DataEntityModels.IEntity;

namespace DALI.DesignBrief.Repositories
{
    public abstract class Repository : IDisposable
    {
        protected IDesignBriefDataEntityModelContext _Context;
        protected string _BaseUrl = string.Empty;
        protected string _FileFolder = string.Empty;

        public Repository() : this(new DesignBriefDataEntityModels())
        {

        }

        public Repository(IDesignBriefDataEntityModelContext context)
        {
            _Context = context;
            _BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
            _FileFolder = ConfigurationManager.AppSettings["FileFolder"];
        }

        public int GetNextIdForEntity<TIdEntity>(IEntity entity)
        {
            return entity.GetNextId(_Context);
        }

        public int GetNextIdForEntity<TIdEntity>(IEntity entity, IDesignBriefDataEntityModelContext context)
        {
            return entity.GetNextId(context);
        }

        public async Task<List<DesignBriefLevelPropertyModel>> GetProperties()
        {
            List<DesignBriefLevelPropertyModel> models = new List<DesignBriefLevelPropertyModel>();

            var entities = await _Context.DesignBriefPolicyRequirementLevels.SelectMany(e => e.DesignBriefPolicyRequirementLevelProperties).ToListAsync();

            foreach (var p in entities)
            {
                var model = new DesignBriefLevelPropertyModel()
                {
                    Id = p.Id,
                    LevelId = p.LevelId.Value,
                    Description = p.Description,
                    Sequence = p.Sequence
                };

                models.Add(model);
            }

            return models.OrderBy(p => p.Sequence).ToList();
        }

        public async Task<List<DesignBriefLevelPropertyModel>> GetProperties(int levelId)
        {
            List<DesignBriefLevelPropertyModel> models = new List<DesignBriefLevelPropertyModel>();

            var entities = await _Context.DesignBriefPolicyRequirementLevels.ToListAsync();

            var entity = entities.SingleOrDefault(e => e.ID == levelId);

            if (null != entity)
            {
                foreach (var p in entity.DesignBriefPolicyRequirementLevelProperties)
                {
                    var model = new DesignBriefLevelPropertyModel()
                    {
                        Id = p.Id,
                        LevelId = p.LevelId.Value,
                        Description = p.Description,
                        Sequence = p.Sequence
                    };

                    models.Add(model);
                }

                return models.OrderBy(p => p.Sequence).ToList();
            }

            return models;
        }

        public async Task<int> Save()
        {
            int result = 0;

            await _Context.SaveChangesAsync();

            result = 1;

            return result;
        }

        protected DbConnection GetConnection()
        {
            return ((DesignBriefDataEntityModels)_Context).Connection;
        }

        public async Task<int> IsGranted(string userName, int designBriefId, int chapterId)
        {
            var result = 0;

            if (chapterId > 0)
            {
                var topic = await _Context.DesignBriefTopics.SingleOrDefaultAsync(e => e.LiorId == chapterId && e.DesignBriefId == designBriefId);
                if (topic != null)
                {
                    if (string.Compare(topic.Owner, userName, true) == 0)
                        result = 1;

                    if (string.Compare(topic.Owner, "unknown", true) == 0)
                        result = 2;
                }
            }
            else
                result = 2;

            return result;
        }

        public async Task<bool> CanEdit(string userName, int designBriefId)
        {
            var result = false;

            var topics = await _Context.DesignBriefTopics.Where(e => e.DesignBriefId == designBriefId && string.Compare(userName, e.Owner, true) == 0 && e.InUse == true).ToArrayAsync();

            result = topics.Count() > 0;

            return result;
        }

        protected DesignBriefDataEntityModels NewContext()
        {
            return new DesignBriefDataEntityModels();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_Context != null)
                {
                    ((DesignBriefDataEntityModels)_Context).Dispose();
                    _Context = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
