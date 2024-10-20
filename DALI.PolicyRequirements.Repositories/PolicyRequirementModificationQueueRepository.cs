using DALI.PolicyRequirements.DataEntityModels;
using DALI.PolicyRequirements.DomainModels;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALI.Enums;
using DALI.Common.Helpers;

namespace DALI.PolicyRequirements.Repositories
{
    public class PolicyRequirementModificationQueueRepository : Repository
    {
        public PolicyRequirementModificationQueueRepository(IPolicyRequirementDataEntityModelContext context) : base(context)
        {
        }

        private PolicyRequirementModificationQueue MapToEntity(PolicyRequirementModificationModel model)
        {
            var entity = new PolicyRequirementModificationQueue();

            entity.Id = model.Id;
            entity.LocalAuthorityID = model.LocalAuthority.Id;
            entity.Description = model.Description;
            entity.ChapterID = model.Chapter.Id;
            entity.LocationID = model.Location.Id;
            entity.LevelID = model.Level.Id;
            entity.AreaID = model.Area.Id;
            entity.SubjectID = model.Subject.Id;
            entity.ChildSubjectID = model.ChildSubject.Id;
            entity.IsActive = model.Active ? 1 : 0;
            entity.CreatedBy = model.CreatedBy;
            entity.CreatedDate = model.CreatedDate;
            entity.ModifiedBy = model.ModifiedBy;
            entity.ModifiedDate = model.ModifiedDate;
            entity.VersionId = model.VersionId;
            entity.Owner = model.Owner;
            entity.GroupIndex = model.GroupIndex;
            entity.VersionId = model.VersionId;

            return entity;
        }

        private PolicyRequirementModificationModel MapToModel(PolicyRequirementModificationQueue entity)
        {
            var model = new PolicyRequirementModificationModel();

            var localAuthority = _Context.PolicyRequirementLocalAuthorities.SingleOrDefault(e => e.Id == entity.LocalAuthorityID.Value);
            var chapter = _Context.PolicyRequirementChapters.SingleOrDefault(e => e.ID == entity.ChapterID.Value && (e.VersionId == entity.VersionId || e.VersionId == -1));
            var location = _Context.PolicyRequirementLocations.SingleOrDefault(e => e.Id == entity.LocationID);
            var level = _Context.PolicyRequirementLevels.SingleOrDefault(e => e.ID == entity.LevelID.Value);
            var area = _Context.PolicyRequirementAreas.SingleOrDefault(e => e.ID == entity.AreaID.Value && (e.VersionId == entity.VersionId || e.VersionId == -1));
            var subject = _Context.PolicyRequirementSubjects.SingleOrDefault(e => e.ID == entity.SubjectID.Value && (e.VersionId == entity.VersionId || e.VersionId == -1));
            var childSubject = _Context.PolicyRequirementSubjects.SingleOrDefault(e => e.ID == entity.ChildSubjectID.Value && (e.VersionId == entity.VersionId || e.VersionId == -1));

            if (chapter == null || level == null || area == null || subject == null)
                throw new Exception("MainPath is not found");

            var currentVersion = _Context.PolicyRequirements.SingleOrDefault(e => e.ID == entity.Id && e.VersionId == entity.VersionId);


            // Fill model properties
            model.Id = entity.Id;
            model.Description = entity.Description;
            model.CurrentDescription = currentVersion != null ? currentVersion.Description : string.Empty;

            model.LocalAuthority.Id = localAuthority.Id;
            model.LocalAuthority.Description = localAuthority.Description;

            model.Chapter.Id = entity.ChapterID ?? 0;
            model.Chapter.ChapterNumber = chapter.ChapterNumber;
            model.Chapter.Active = (chapter.IsActive == 1);
            model.Chapter.Owner = chapter.Owner;
            model.Chapter.VersionId = chapter.VersionId;
            model.Chapter.Description = chapter.Description;

            if (entity.LocationID.HasValue && entity.LocationID != Guid.Empty)
            {
                model.Location.Id = location.Id;
                model.Location.Description = location.Description;
            }

            model.Level.Id = level.ID;
            model.Level.Active = (level.IsActive == 1);
            model.Level.Name = level.Name;
            model.Level.Position = level.Position;
            model.Level.Description = level.Description;

            model.Area.Id = area.ID;
            model.Area.Active = (area.IsActive == 1);
            model.Area.Description = area.Description;
            model.Area.VersionId = area.VersionId;

            model.Subject.Id = subject.ID;
            model.Subject.Active = (subject.IsActive == 1);
            model.Subject.Description = subject.Description;
            model.Subject.VersionId = subject.VersionId;

            if (entity.ChildSubjectID.HasValue && entity.ChildSubjectID > 0)
            {
                model.ChildSubject.Id = childSubject.ID;
                model.ChildSubject.Description = childSubject.Description;
                model.ChildSubject.Active = (childSubject.IsActive == 1);
                model.ChildSubject.Description = childSubject.Description;
                model.ChildSubject.VersionId = childSubject.VersionId;
            }

            model.CreatedBy = entity.CreatedBy;
            model.CreatedDate = entity.CreatedDate;
            model.ModifiedBy = entity.ModifiedBy;
            model.ModifiedDate = entity.ModifiedDate;
            model.GroupIndex = entity.GroupIndex;
            model.Active = entity.IsActive == 1;

            model.Owner = entity.Owner;
            model.VersionId = entity.VersionId;

            int iAssignmentAttachmentEnum = AssignmentEnum.Attachment.AsInt();
            int iAssignmentSourceEnum = AssignmentEnum.SourceDocument.AsInt();

            model.HasAttachments = entity.PolicyRequirementModificationAssignmentQueue.Where(e => e.PolicyRequirementId == entity.Id && e.VersionId == entity.VersionId && e.AssignmentTypeId == iAssignmentAttachmentEnum).Count() > 0 ? 1 : 0;
            model.HasSourceReferences = entity.PolicyRequirementModificationAssignmentQueue.Where(e => e.PolicyRequirementId == entity.Id && e.VersionId == entity.VersionId && e.AssignmentTypeId == iAssignmentSourceEnum).Count() > 0 ? 1 : 0;

            return model;
        }

        public async Task<PolicyRequirementModificationModel> New(bool withNewId)
        {
            var model = new PolicyRequirementModificationModel();

            if (withNewId)
            {
                int nextId = await GetNextIdForEntity(new PolicyRequirement());

                model.Id = nextId;
            }

            return model;
        }

        private async Task<PolicyRequirementPublishQueue> QueueForPublication(int id)
        {
            var entity = await _Context.PolicyRequirementPublishQueues.SingleOrDefaultAsync(e => e.ID == id && e.VersionId == null).ConfigureAwait(true);

            return entity;
        }

        private async Task<PolicyRequirementModificationQueue> GetOneEntityAsync(int id, int versionId)
        {
            var entity = await _Context.PolicyRequirementModificationQueues.SingleOrDefaultAsync(e => e.Id == id && e.VersionId == versionId);

            return entity;
        }

        public async Task<int> Add(PolicyRequirementModificationModel model)
        {
            int result = 0;

            await Task.Run(() =>
            {
                var entity = MapToEntity(model);

                _Context.PolicyRequirementModificationQueues.AddObject(entity);

                result = 1;
            });

            return result;
        }

        public async Task<List<PolicyRequirementModificationModel>> GetAll(int versionId, string owner)
        {
            var entities = _Context.PolicyRequirementModificationQueues.Where(e => e.VersionId == versionId && e.Owner == owner);

            var models = new List<PolicyRequirementModificationModel>();

            foreach (var e in entities)
            {
                try
                {
                    if (models.SingleOrDefault(m => m.Id == e.Id) == null)
                    {
                        var model = MapToModel(e);

                        await SetPublicationQueueInfo(model, versionId);

                        models.Add(model);
                    }
                }
                catch (Exception err)
                {
                    var msg = err.Message;
                }
            }
            return models.OrderByDescending(e => e.ModifiedDate).ToList();
        }

        private async Task<int> SetPublicationQueueInfo(PolicyRequirementModificationModel model, int versionId)
        {
            int result = 0;

            var qpEntity = await QueueForPublication(model.Id);

            if (qpEntity == null)
            {
                var comments = await _Context.PolicyRequirementPublishingAdditionalInfoes.Where(e => e.PolicyRequirementId == model.Id && e.ActionId == PublishingDecisionEnum.Rejected && e.VersionId == versionId).ToListAsync().ConfigureAwait(true);
                var comment = comments.OrderBy(e => e.Id).LastOrDefault();

                if (comment != null)
                {
                    model.PublicationStatus = PublishingDecisionEnum.Rejected;
                    model.RejectMessage = comment.Description;
                    model.RejectedBy = comment.CreatedBy;
                }
                else
                {
                    model.PublicationStatus = PublishingDecisionEnum.Undescided;
                }

                model.QueuedForPublication = false;
            }

            if (qpEntity != null)
            {
                model.QueuedForPublication = true;
                model.PublicationStatus = (qpEntity.CanPublish == true) ? PublishingDecisionEnum.Approved : PublishingDecisionEnum.Parked;
            }

            result = 1;

            return result;
        }

        public async Task<PolicyRequirementModificationModel> GetOneModelAsync(int id, int versionId)
        {
            var entity = await GetOneEntityAsync(id, versionId);

            if (entity != null)
            {
                var model = MapToModel(entity);

                var result = await SetPublicationQueueInfo(model, versionId);

                return model;
            }

            return default(PolicyRequirementModificationModel);
        }

        public async Task<int> Update(PolicyRequirementModificationModel model)
        {
            int result = 0;

            var entity = await GetOneEntityAsync(model.Id, model.VersionId);

            if (entity != null)
            {
                if (!string.IsNullOrEmpty(model.Description))
                    entity.Description = model.Description;

                entity.ModifiedBy = model.ModifiedBy;
                entity.ModifiedDate = DateTime.Now;

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
                _Context.PolicyRequirementModificationQueues.DeleteObject(entity);

                result = 1;
            }

            return result;
        }

        public async Task<int[]> AssignNewOwner(int chapterId, string oldOwner, string newOwner, int versionId)
        {
            var entities = await _Context.PolicyRequirementModificationQueues.Where(e => string.Compare(e.Owner, oldOwner, true) == 0 && e.ChapterID == chapterId && e.VersionId == versionId).ToListAsync();

            foreach (var entity in entities)
            {
                if (!entity.Owner.Equals(newOwner, StringComparison.OrdinalIgnoreCase))
                {
                    entity.Owner = newOwner;
                }
            }

            return entities.Select(e => e.Id).ToArray();
        }

        public async Task<bool> ChapterIsQueued(int id, int versionId)
        {
            var count = await _Context.PolicyRequirementModificationQueues.Where(e => e.ChapterID == id && e.VersionId == versionId).CountAsync();

            return count > 0;
        }

        public async Task<bool> AreaIsQueued(int id, int versionId)
        {
            var count = await _Context.PolicyRequirementModificationQueues.Where(e => e.AreaID == id && e.VersionId == versionId).CountAsync();

            return count > 0;
        }

        public async Task<bool> SubjectIsQueued(int id, int versionId)
        {
            var count = await _Context.PolicyRequirementModificationQueues.Where(e => e.SubjectID == id && e.VersionId == versionId).CountAsync();

            return count > 0;
        }
    }
}
