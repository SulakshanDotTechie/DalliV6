using DALI.PolicyRequirements.DataEntityModels;
using DALI.PolicyRequirements.DomainModels;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.Common;
using System.Security.Principal;

namespace DALI.PolicyRequirements.Repositories
{
    public abstract class Repository : IDisposable
    {
        protected IPolicyRequirementDataEntityModelContext _Context;
        protected string _BaseUrl = string.Empty;
        protected string _FileFolder = string.Empty;
        protected DbTransaction _Trans;

        public Repository()
            : this(new PolicyRequirementDataEntityModels())
        {
        }

        public Repository(IPolicyRequirementDataEntityModelContext context)
        {
            _Context = context;
            //_BaseUrl = (string)ConfigurationManager.AppSettings["BaseUrl"];
            //_FileFolder = (string)ConfigurationManager.AppSettings["FileFolder"];
            _BaseUrl = "test";
            _FileFolder = "test2";
            _Trans = GetConnection().BeginTransaction();
        }

        public Task<int> GetNextIdForEntity<TIdEntity>(TIdEntity entity)
        {
            return ((IEntity)entity).GetNextIdAsync(_Context);
        }

        public Task<int> GetNextIdForEntity<TIdEntity>(TIdEntity entity, IPolicyRequirementDataEntityModelContext context)
        {
            return ((IEntity)entity).GetNextIdAsync(context);
        }


        public async Task<int> Save()
        {
            int result = 0;

            int isSaved = await _Context.SaveChangesAsync();

            if (isSaved > 0)
            {
                result = 1;
            }

            return result;
        }


        protected PolicyRequirement MapToEntity(PolicyRequirementModel model)
        {
            var entity = new PolicyRequirement();

            entity.LocalAuthorityID = model.LocalAuthority.Id;
            entity.ID = model.Id;
            entity.Description = model.Description;
            entity.ChapterID = model.Chapter.Id;
            entity.LevelID = model.Level.Id;
            entity.LocationID = model.Location.Id;
            entity.AreaID = model.Area.Id;
            entity.SubjectID = model.Subject.Id;
            entity.ChildSubjectID = model.ChildSubject.Id;

            entity.CreatedBy = model.CreatedBy;
            entity.CreatedDate = model.CreatedDate;
            entity.ModifiedBy = model.ModifiedBy;
            entity.ModifiedDate = model.ModifiedDate;
            entity.Owner = model.Owner;
            entity.GroupIndex = model.GroupIndex;
            entity.OrderIndex = model.OrderIndex;
            entity.IsActive = model.Active ? 1 : 0;
            entity.VersionId = model.VersionId;

            return entity;
        }

        protected PolicyRequirementModel MapToModel(PolicyRequirement entity)
        {
            var model = new PolicyRequirementModel();

            // Fill model properties
            model.Id = entity.ID;
            model.Description = entity.Description;

            if (entity.LocalAuthorityID.HasValue && entity.LocalAuthorityID != Guid.Empty)
            {
                model.LocalAuthority.Id = entity.PolicyRequirementLocalAuthority.Id;
                model.LocalAuthority.Description = entity.PolicyRequirementLocalAuthority.Description;
            }

            model.Chapter.Id = entity.ChapterID ?? 0;
            model.Chapter.ChapterNumber = entity.PolicyRequirementChapter.ChapterNumber;
            model.Chapter.Active = (entity.PolicyRequirementChapter.IsActive == 1);
            model.Chapter.Owner = entity.PolicyRequirementChapter.Owner;
            model.Chapter.Description = entity.PolicyRequirementChapter.Description;

            model.Level.Id = entity.LevelID ?? 0;
            model.Level.Active = (entity.PolicyRequirementLevel.IsActive == 1);
            model.Level.Name = entity.PolicyRequirementLevel.Name;
            model.Level.Position = entity.PolicyRequirementLevel.Position;
            model.Level.Description = entity.PolicyRequirementLevel.Description;

            if (entity.LocationID.HasValue && entity.LocationID != Guid.Empty)
            {
                model.Location.Id = entity.PolicyRequirementLocation.Id;
                model.Location.Description = entity.PolicyRequirementLocation.Description;
                model.Location.OrderIndex = entity.PolicyRequirementLocation.OrderIndex ?? default(int);
            }
            else
            {
                model.Location.Description = " ";
            }

            model.Area.Id = entity.AreaID ?? 0;
            model.Area.Active = (entity.PolicyRequirementArea.IsActive == 1);
            model.Area.Description = entity.PolicyRequirementArea.Description;
            model.Area.VersionId = entity.PolicyRequirementArea.VersionId;
            model.Area.FetchByDefault = entity.PolicyRequirementArea.FetchByDefault == 1;
            model.Area.IsTownSpecific = entity.PolicyRequirementArea.IsTownSpecific ?? default(bool);

            model.Subject.Id = entity.SubjectID ?? 0;
            model.Subject.Active = (entity.PolicyRequirementSubject.IsActive == 1);
            model.Subject.Description = entity.PolicyRequirementSubject.Description;
            model.Subject.VersionId = entity.PolicyRequirementSubject.VersionId;
            model.Subject.FetchByDefault = entity.PolicyRequirementSubject.FetchByDefault == 1;
            model.Subject.IsTownSpecific = entity.PolicyRequirementSubject.IsTownSpecific ?? default(bool);

            if (entity.ChildSubjectID > 0)
            {
                model.ChildSubject.Id = entity.ChildSubjectID ?? 0;
                model.ChildSubject.Description = entity.PolicyRequirementChildSubjects.Description;
                model.ChildSubject.VersionId = entity.PolicyRequirementChildSubjects.VersionId;
            }
            else
            {
                model.ChildSubject.Description = " ";
            }

            model.CreatedBy = entity.CreatedBy;
            model.CreatedDate = entity.CreatedDate;
            model.ModifiedBy = entity.ModifiedBy;
            model.ModifiedDate = entity.ModifiedDate;
            model.ExpirationDate = entity.ExpirationDate;

            model.GroupIndex = entity.GroupIndex;
            model.OrderIndex = entity.OrderIndex;
            model.Active = entity.IsActive == 1;
            model.Owner = entity.Owner;

            model.VersionId = entity.VersionId;
            model.UniqueID = entity.UniqueID;

            model.HasAttachments = entity.PolicyRequirementAssignedAttachment.Where(e => e.PolicyRequirementId == entity.ID && e.VersionId == entity.VersionId).Count() > 0 ? 1 : 0;
            model.HasSourceReferences = entity.PolicyRequirementAssignedSources.Where(e => e.PolicyRequirementId == entity.ID && e.VersionId == entity.VersionId).Count() > 0 ? 1 : 0;

            return model;
        }

        private List<PolicyRequirementModel> OrderBy(IEnumerable<PolicyRequirementModel> models)
        {
            return models.OrderBy(m => m.Chapter.FullChapterDescription).ThenBy(m => m.Level.Position).ThenBy(e => e.OrderIndex).ToList();
        }

        private IQueryable<PolicyRequirement> GetAllEntities()
        {
            return _Context.PolicyRequirements;
        }

        public async Task<List<PolicyRequirementModel>> GetPolicyRequirements(int versionId)
        {
            var entities = await GetAllEntities().Where(e => e.VersionId == versionId).ToListAsync();

            var models = entities.Select(r => MapToModel(r));

            return OrderBy(models);
        }

        public async Task<List<PolicyRequirementModel>> GetPolicyRequirements(int[] ids, int versionId)
        {
            var entities = await GetAllEntities().Where(e => ids.Contains(e.ID) && e.VersionId == versionId).ToListAsync();

            var models = entities.Select(r => MapToModel(r));

            return OrderBy(models);
        }

        public async Task<List<PolicyRequirementModel>> GetPolicyRequirements(int chapterId, int versionId)
        {
            var entities = await GetAllEntities().Where(e => e.ChapterID == chapterId && e.VersionId == versionId).ToListAsync();

            var models = entities.Select(r => MapToModel(r));

            return OrderBy(models);
        }

        protected PolicyRequirementDataEntityModels NewContext()
        {
            return new PolicyRequirementDataEntityModels();
        }

        public DbConnection GetConnection()
        {
            return ((PolicyRequirementDataEntityModels)_Context).Connection;
        }

        public void StartTransaction()
        {
            try
            {
                GetConnection().Open();
                _Trans = GetConnection().BeginTransaction();
            }
            catch (Exception error)
            {
                var err = error;
            }
        }

        public void CommitTransaction()
        {
            if (_Trans != null)
                _Trans.Commit();
        }

        public void RollbackTransaction()
        {
            if (_Trans != null)
                _Trans.Rollback();
        }

        public void ResetNextIdCounter<T>(int startValue) where T : IEntity, new()
        {
            string tableName = new T().TableName;
            var entity = _Context.TT_SYS_GENID.SingleOrDefault(e => string.Compare(e.TT_TABLENAME, tableName, true) == 0);

            if (entity != null)
            {
                entity.TT_LASTGEN_ID = startValue;
                _Context.SaveChanges();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_Context != null)
                {
                    ((PolicyRequirementDataEntityModels)_Context).Dispose();
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
