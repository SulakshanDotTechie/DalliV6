using DALI.DesignBrief.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DALI.DesignBrief.DataEntityModels;
using System.Transactions;
using System.IO;
using DALI.DesignBrief.Interfaces;
using ChinhDo.Transactions;
using System.Data.Entity;
using ChinhDo.Transactions.FileManager;

namespace DALI.DesignBrief.Repositories
{
    public class DesignBriefPolicyRequirementRepository : Repository
    {
        private const string BasePath = "{0}Files{3}Attachments{3}{1}{3}{2}";
        private IDesignBriefPolicyRequirementRepositoryHelper _helper;

        public DesignBriefPolicyRequirementRepository(IDesignBriefPolicyRequirementRepositoryHelper helper) : base()
        {
            _helper = helper;
        }

        public DesignBriefPolicyRequirementRepository(IDesignBriefPolicyRequirementRepositoryHelper helper, IDesignBriefDataEntityModelContext context) : base(context)
        {
            _helper = helper;
        }

        private DesignBriefItem MapToEntity(DesignBriefPolicyRequirementModel model)
        {
            DesignBriefItem entity = new DesignBriefItem();

            entity.Id = model.Id;
            entity.LocalAuthorityId = model.LocalAuthorityId;
            entity.UniqueId = model.UniqueID ?? default(Guid);
            entity.LocationId = model.LocationId;
            entity.Description = model.Description;
            entity.ChapterId = model.ChapterId;
            entity.LocationId = model.LocationId;
            entity.LevelId = model.LevelId;
            entity.AreaId = model.AreaId;
            entity.SubjectId = model.SubjectId;
            entity.ChildSubjectId = model.ChildSubjectId;
            entity.PolicyRequirementId = model.PolicyRequirementId;
            entity.PolicyRequirementUniqueID = model.PolicyRequirementUniqueId;
            entity.GroupIndex = model.GroupIndex;
            entity.CreatedDate = model.CreatedDate;
            entity.ModifiedDate = model.ModifiedDate;
            entity.CreatedBy = model.CreatedBy;
            entity.ModifiedBy = model.ModifiedBy;
            entity.VersionId = model.VersionId;
            entity.OrderIndex = model.OrderIndex;

            return entity;
        }

        private DesignBriefPolicyRequirementModel MapToModel(DesignBriefItem entity)
        {
            DesignBriefPolicyRequirementModel model = new DesignBriefPolicyRequirementModel();

            model.Id = entity.Id;
            model.LocalAuthorityId = entity.LocalAuthorityId ?? default(Guid);
            model.UniqueID = entity.UniqueId;
            model.Description = entity.Description;
            model.ChapterId = entity.ChapterId ?? default(int);
            model.LocationId = entity.LocationId ?? default(Guid);
            model.LevelId = entity.LevelId ?? default(int);
            model.AreaId = entity.AreaId ?? default(int);
            model.SubjectId = entity.SubjectId ?? default(int);
            model.ChildSubjectId = entity.ChildSubjectId ?? default(int);
            model.PolicyRequirementId = entity.PolicyRequirementId ?? default(int);
            model.PolicyRequirementUniqueId = entity.PolicyRequirementUniqueID ?? Guid.Empty;
            model.GroupIndex = entity.GroupIndex ?? default(int);
            model.CreatedDate = entity.CreatedDate ?? default(DateTime);
            model.ModifiedDate = entity.ModifiedDate ?? default(DateTime);
            model.CreatedBy = entity.CreatedBy;
            model.ModifiedBy = entity.ModifiedBy;
            model.VersionId = entity.VersionId;
            model.OrderIndex = entity.OrderIndex;

            return model;
        }

        private async Task<DesignBriefPolicyRequirementModel[]> GetModels(int designBriefId, bool handleEmptyChapters)
        {
            var models = new List<DesignBriefPolicyRequirementModel>();

            var topics = await _Context.DesignBriefTopics.Where(e => e.DesignBriefId == designBriefId && e.InUse == true).OrderBy(e => e.ChapterNumber).ToArrayAsync();

            foreach (var topic in topics)
            {
                try
                {
                    var entities = _Context.DesignBriefItems.Where(e => e.ChapterId == topic.LiorId && e.DesignBriefId == designBriefId).ToArray();

                    if (entities.Count() == 0 && handleEmptyChapters)
                    {
                        DesignBriefPolicyRequirementModel model = MapToModel(new DesignBriefItem());

                        model.Chapter.Id = (int)topic.LiorId;
                        model.Id = 10000 + model.Chapter.Id;
                        model.PolicyRequirementId = model.Id;
                        model.ChapterId = model.Chapter.Id;
                        model.Chapter.DesignBriefId = designBriefId;
                        model.Chapter.ChapterNumber = topic.ChapterNumber;
                        model.Chapter.Description = topic.Description;
                        model.Chapter.Owner = "unknown";
                        model.Chapter.OwnerEmailAddress = string.Empty;
                        model.Chapter.VersionId = topic.VersionId;

                        model.CreatedDate = DateTime.Now;
                        model.ModifiedDate = model.CreatedDate;
                        model.CreatedBy = "unknown";
                        model.ModifiedBy = "unknown";

                        model.Level.Id = model.PolicyRequirementId;
                        model.Level.Position = Resources.Localization.FakePosition;
                        model.Level.Name = Resources.Localization.ToFurtherDefine;
                        model.Level.Description = Resources.Localization.ToFurtherDefine;

                        model.Location.Description = Resources.Localization.ToFurtherDefine;

                        model.Area.Id = model.PolicyRequirementId;
                        model.Area.DesignBriefId = designBriefId;
                        model.Area.Description = Resources.Localization.ToFurtherDefine;

                        model.Subject.Id = model.PolicyRequirementId;
                        model.Subject.DesignBriefId = designBriefId;
                        model.Subject.Description = Resources.Localization.ToFurtherDefine;

                        model.ChildSubject.Description = Resources.Localization.ToFurtherDefine;

                        model.OrderIndex = model.PolicyRequirementId;
                        model.GroupIndex = model.PolicyRequirementId;

                        model.Description = string.Format(Resources.Localization.TopicHasNoContent, topic.Owner);

                        if (models.SingleOrDefault(e => e.Id == model.Id) == null)
                        {
                            models.Add(model);
                        }
                    }
                    else
                    {
                        foreach (DesignBriefItem entity in entities)
                        {
                            DesignBriefPolicyRequirementModel model = MapToModel(entity);

                            model.HasAttachments = entity.DesignBriefPolicyRequirementAssignedAttachments.Where(e => e.DesignBriefId == entity.DesignBriefId && e.DesignBriefPolicyRequirementId == entity.Id && e.VersionId == entity.VersionId).ToArray().Length > 0 ? 1 : 0;
                            model.HasSourceReferences = entity.DesignBriefPolicyRequirementAssignedSources.Where(e => e.DesignBriefId == entity.DesignBriefId && e.DesignBriefPolicyRequirementId == entity.Id && e.VersionId == entity.VersionId).ToArray().Length > 0 ? 1 : 0;

                            if (entity.LocalAuthorityId.HasValue && entity.LocalAuthorityId != Guid.Empty)
                            {
                                model.LocalAuthority.Id = entity.DesignBriefPolicyRequirementLocalAuthority.Id;
                                model.LocalAuthority.Description = entity.DesignBriefPolicyRequirementLocalAuthority.Description;
                            }

                            model.Chapter.Id = entity.DesignBriefPolicyRequirementChapters.ID;
                            model.Chapter.DesignBriefId = designBriefId;
                            model.Chapter.ChapterNumber = entity.DesignBriefPolicyRequirementChapters.ChapterNumber;
                            model.Chapter.Description = entity.DesignBriefPolicyRequirementChapters.Description;
                            model.Chapter.VersionId = entity.DesignBriefPolicyRequirementChapters.VersionId;

                            model.Chapter.Owner = topic.Owner;
                            model.Chapter.OwnerEmailAddress = topic.OwnerEmailAddress;

                            model.Level.Id = entity.DesignBriefPolicyRequirementLevels.ID;
                            model.Level.Name = entity.DesignBriefPolicyRequirementLevels.Name;
                            model.Level.Description = entity.DesignBriefPolicyRequirementLevels.Description;
                            model.Level.Position = entity.DesignBriefPolicyRequirementLevels.Position;

                            if (entity.LocationId.HasValue && entity.LocationId != Guid.Empty)
                            {
                                model.Location.Id = entity.DesignBriefPolicyRequirementLocation.Id;
                                model.Location.Description = entity.DesignBriefPolicyRequirementLocation.Description;
                            }
                            else
                            {
                                model.Location.Description = " ";
                            }

                            model.Area.Id = entity.DesignBriefPolicyRequirementArea.ID;
                            model.Area.DesignBriefId = designBriefId;
                            model.Area.Description = entity.DesignBriefPolicyRequirementArea.Description;
                            model.Area.VersionId = entity.DesignBriefPolicyRequirementArea.VersionId;

                            model.Subject.Id = entity.DesignBriefPolicyRequirementSubject.ID;
                            model.Subject.DesignBriefId = designBriefId;
                            model.Subject.Description = entity.DesignBriefPolicyRequirementSubject.Description;
                            model.Subject.VersionId = entity.DesignBriefPolicyRequirementSubject.VersionId;

                            if (entity.ChildSubjectId > 0 && entity.DesignBriefPolicyRequirementChildSubject != null)
                            {
                                model.ChildSubject.DesignBriefId = entity.DesignBriefPolicyRequirementChildSubject.DesignBriefId;
                                model.ChildSubject.Id = entity.DesignBriefPolicyRequirementChildSubject.ID;
                                model.ChildSubject.Description = entity.DesignBriefPolicyRequirementChildSubject.Description;
                                model.ChildSubject.VersionId = entity.DesignBriefPolicyRequirementChildSubject.VersionId;
                            }
                            else
                            {
                                model.ChildSubject.Description = " ";
                            }

                            if (entity.DesignBriefPolicyRequirementAssignedStrictnesses != null)
                            {
                                var strictnessEntities = entity.DesignBriefPolicyRequirementAssignedStrictnesses;

                                foreach (var strictness in strictnessEntities)
                                {
                                    model.Strictnesses.Add(MapToModel(strictness.DesignBriefPolicyRequirementStrictness));
                                }
                            }

                            if (models.SingleOrDefault(e => e.Id == model.Id) == null)
                            {
                                models.Add(model);
                            }
                        }
                    }
                }
                catch (Exception error)
                {

                }
            }

            var results = models.OrderBy(m => m.Chapter.FullChapterDescription).ThenBy(m => m.Level.Position).ThenBy(e => e.OrderIndex).ToArray();

            return results;
        }

        public async Task<DesignBriefDetailsModel> GetBy(int designBriefId, bool handleEmptyChapters)
        {
            DesignBriefDetailsModel details = new DesignBriefDetailsModel();

            DesignBriefPolicyRequirementModel[] models = await GetModels(designBriefId, handleEmptyChapters);

            details.DesignBriefId = designBriefId;
            details.Items = models.ToList();

            return details;
        }

        public async Task<int[]> GetByAttachments(int designBriefId, string description)
        {
            var ids = await _Context.DesignBriefPolicyRequirementAssignedAttachments.Where(e => e.DesignBriefId == designBriefId && (e.DesignBriefPolicyRequirementAttachments != null && (e.DesignBriefPolicyRequirementAttachments.Description.Contains(description) || e.DesignBriefPolicyRequirementAttachments.StorageLocation.Contains(description)))).Select(e => e.DesignBriefPolicyRequirementId).Distinct().ToArrayAsync();

            return ids;
        }

        public async Task<int[]> GetBySourceReferences(int designBriefId, string description)
        {
            var ids = await _Context.DesignBriefPolicyRequirementAssignedSources.Where(e => e.DesignBriefId == designBriefId && (e.DesignBriefPolicyRequirementSource != null && (e.DesignBriefPolicyRequirementSource.Description.Contains(description) || e.DesignBriefPolicyRequirementSource.StorageLocation.Contains(description)))).Select(e => e.DesignBriefPolicyRequirementId).Distinct().ToArrayAsync();

            return ids;
        }

        public async Task<List<string>> GetSemiPathKeys(int id)
        {
            var indexList = new List<string>();

            var entites = await _Context.DesignBriefItems.Where(e => e.DesignBriefId == id).ToListAsync();

            var updateEnts = ((DesignBriefDataEntityModels)_Context).LoadedEntities<DesignBriefItem>().Where(e => e.DesignBriefId == id).ToList();

            entites.AddRange(updateEnts);

            indexList.AddRange(entites.Select(e => e.SemiPathKey).Distinct());

            return indexList;
        }

        public async Task<DesignBriefPolicyRequirementModel> GetOne(int designBriefId, int id)
        {
            var models = await GetModels(designBriefId, true);

            var model = models.SingleOrDefault(e => e.Id == id);

            return model;
        }

        public async Task<int> Update(DesignBriefDetailsModel model)
        {
            var result = 0;
            using (var context = NewContext())
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.Parse("60"), TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        var dbfEntity = await _Context.DesignBriefs.SingleOrDefaultAsync(e => e.Id == model.DesignBriefId);

                        if (dbfEntity != null)
                        {
                            foreach (var item in model.Items)
                            {
                                var entity = await context.DesignBriefItems.SingleOrDefaultAsync(e => e.DesignBriefId == model.DesignBriefId && e.Id == item.Id);

                                if (entity != null)
                                {
                                    var assStrs = await context.DesignBriefPolicyRequirementAssignedStrictnesses.Where(e => e.DesignBriefId == model.DesignBriefId && e.DesignBriefPolicyRequirementId == item.Id).ToArrayAsync();
                                    foreach (var str in assStrs)
                                    {
                                        context.DesignBriefPolicyRequirementAssignedStrictnesses.DeleteObject(str);
                                    }

                                    var assSrcs = await context.DesignBriefPolicyRequirementAssignedSources.Where(e => e.DesignBriefId == model.DesignBriefId && e.DesignBriefPolicyRequirementId == item.Id).ToArrayAsync();
                                    foreach (var src in assSrcs)
                                    {
                                        context.DesignBriefPolicyRequirementAssignedSources.DeleteObject(src);
                                    }

                                    var assAtts = await context.DesignBriefPolicyRequirementAssignedAttachments.Where(e => e.DesignBriefId == model.DesignBriefId && e.DesignBriefPolicyRequirementId == item.Id).ToArrayAsync();
                                    foreach (var att in assAtts)
                                    {
                                        context.DesignBriefPolicyRequirementAssignedAttachments.DeleteObject(att);
                                    }

                                    await context.SaveChangesAsync();

                                    if (entity.ChapterId != item.ChapterId)
                                    {
                                        var entChapter = await context.DesignBriefPolicyRequirementChapters.SingleOrDefaultAsync(e => e.DesignBriefId == model.DesignBriefId && e.ID == item.ChapterId);

                                        if (entChapter == null)
                                        {
                                            // We have to fix this. This current version of a PvE
                                            // is based on using the LiorId. But what to do when adding
                                            // a new Chapter in the PvE which does not exist in the Lior
                                            // and is only part of a PvE!!!
                                            var entTopic = await context.DesignBriefTopics.SingleOrDefaultAsync(e => e.DesignBriefId == model.DesignBriefId && e.LiorId == item.ChapterId);

                                            if (entTopic != null)
                                            {
                                                var chrEntity = new DesignBriefPolicyRequirementChapter();
                                                chrEntity.ID = (int)entTopic.LiorId;
                                                chrEntity.ChapterNumber = entTopic.ChapterNumber;
                                                chrEntity.Description = entTopic.Description;
                                                chrEntity.VersionId = entTopic.VersionId;
                                                chrEntity.DesignBriefId = model.DesignBriefId;
                                                context.DesignBriefPolicyRequirementChapters.AddObject(chrEntity);
                                            }
                                        }

                                        entity.ChapterId = item.ChapterId;
                                    }

                                    entity.LevelId = item.LevelId;

                                    if (entity.LocationId != item.LocationId)
                                    {
                                        var entLocation = await context.DesignBriefPolicyRequirementLocations.SingleOrDefaultAsync(e => e.Id == item.LocationId);

                                        if (entLocation == null)
                                        {
                                            var location = await _helper.GetLocation(item.LocationId);

                                            if (location != null)
                                            {
                                                var locationEntity = new DesignBriefPolicyRequirementLocation();
                                                locationEntity.Id = location.Id;
                                                locationEntity.Description = location.Description;
                                                context.DesignBriefPolicyRequirementLocations.AddObject(locationEntity);
                                            }
                                        }

                                        entity.LocationId = item.LocationId;
                                    }

                                    if (entity.AreaId != item.AreaId)
                                    {
                                        var entArea = await context.DesignBriefPolicyRequirementAreas.SingleOrDefaultAsync(e => e.DesignBriefId == model.DesignBriefId && e.ID == item.AreaId);

                                        if (entArea == null)
                                        {
                                            var area = await _helper.GetArea(item.AreaId);

                                            if (area != null)
                                            {
                                                var areaEntity = new DesignBriefPolicyRequirementArea();
                                                areaEntity.ID = area.Id;
                                                areaEntity.Description = area.Description;
                                                areaEntity.VersionId = area.VersionId;
                                                areaEntity.DesignBriefId = model.DesignBriefId;
                                                context.DesignBriefPolicyRequirementAreas.AddObject(areaEntity);
                                            }
                                        }

                                        entity.AreaId = item.AreaId;
                                    }

                                    if (entity.SubjectId != item.SubjectId)
                                    {
                                        var subjectEntity = await context.DesignBriefPolicyRequirementSubjects.SingleOrDefaultAsync(e => e.DesignBriefId == model.DesignBriefId && e.ID == item.SubjectId);

                                        if (subjectEntity == null)
                                        {
                                            var subject = await _helper.GetSubject(item.SubjectId);

                                            if (subject != null)
                                            {
                                                var subjEntity = new DesignBriefPolicyRequirementSubject();
                                                subjEntity.ID = subject.Id;
                                                subjEntity.Description = subject.Description;
                                                subjEntity.VersionId = subject.VersionId;
                                                subjEntity.DesignBriefId = model.DesignBriefId;
                                                context.DesignBriefPolicyRequirementSubjects.AddObject(subjEntity);
                                            }
                                        }

                                        entity.SubjectId = item.SubjectId;
                                    }

                                    if (entity.ChildSubjectId != item.ChildSubjectId)
                                    {
                                        var childSubjectEntity = await context.DesignBriefPolicyRequirementSubjects.SingleOrDefaultAsync(e => e.DesignBriefId == model.DesignBriefId && e.ID == item.ChildSubjectId);

                                        if (childSubjectEntity == null)
                                        {
                                            var childSubject = await _helper.GetChildSubject(item.ChildSubjectId);

                                            if (childSubject != null)
                                            {
                                                var childSubjEntity = new DesignBriefPolicyRequirementSubject();
                                                childSubjEntity.ID = childSubject.Id;
                                                childSubjEntity.Description = childSubject.Description;
                                                childSubjEntity.VersionId = childSubject.VersionId;
                                                childSubjEntity.DesignBriefId = model.DesignBriefId;
                                                context.DesignBriefPolicyRequirementSubjects.AddObject(childSubjEntity);
                                            }
                                        }

                                        entity.ChildSubjectId = item.ChildSubjectId;
                                    }

                                    entity.Description = item.Description;
                                    entity.ModifiedDate = item.ModifiedDate;
                                    entity.ModifiedBy = item.ModifiedBy;

                                    ProcessSourceReferences(item, model.DesignBriefId, context);
                                    ProcessAttachments(item, model.DesignBriefId, dbfEntity.Code, context);
                                    ProcessStrictness(item, model.DesignBriefId, context);

                                    item.HasAttachments = item.Attachments.ToArray().Length > 0 ? 1 : 0;
                                    item.HasSourceReferences = item.SourceDocuments.ToArray().Length > 0 ? 1 : 0;

                                    await context.SaveChangesAsync();
                                }
                            }
                        }

                        scope.Complete();

                        result = 1;
                    }
                    catch (Exception error)
                    {
                        scope.Dispose();

                        throw error;
                    }
                }
            }

            return result;
        }

        public async Task<int> Add(DesignBriefDetailsModel model)
        {
            var result = 0;

            using (var context = NewContext())
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.Parse("60"), TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        var dbfEntity = context.DesignBriefs.SingleOrDefault(e => e.Id == model.DesignBriefId);

                        if (dbfEntity != null)
                        {
                            foreach (var item in model.Items)
                            {
                                var entity = MapToEntity(item);

                                entity.DesignBriefId = model.DesignBriefId;
                                entity.UniqueId = Guid.NewGuid();
                                entity.Id = GetNextIdForEntity<DesignBriefItem>(entity, context);

                                item.Id = entity.Id;
                                item.UniqueID = entity.UniqueId;

                                context.DesignBriefItems.AddObject(entity);

                                DesignBriefPolicyRequirementChapter chrEntity = context.LoadedEntities<DesignBriefPolicyRequirementChapter>().SingleOrDefault(e => e.ID == item.ChapterId && e.VersionId == item.VersionId && e.DesignBriefId == model.DesignBriefId);
                                if (chrEntity == null)
                                {
                                    chrEntity = context.DesignBriefPolicyRequirementChapters.SingleOrDefault(e => e.ID == item.ChapterId && e.VersionId == item.VersionId && e.DesignBriefId == model.DesignBriefId);
                                    if (chrEntity == null)
                                    {
                                        chrEntity = new DesignBriefPolicyRequirementChapter();
                                        chrEntity.ID = item.Chapter.Id;
                                        chrEntity.ChapterNumber = item.Chapter.ChapterNumber;
                                        chrEntity.Description = item.Chapter.Description;
                                        chrEntity.VersionId = item.Chapter.VersionId;
                                        chrEntity.DesignBriefId = model.DesignBriefId;
                                        context.DesignBriefPolicyRequirementChapters.AddObject(chrEntity);

                                        await context.SaveChangesAsync();
                                    }
                                }

                                DesignBriefPolicyRequirementArea araEntity = context.LoadedEntities<DesignBriefPolicyRequirementArea>().SingleOrDefault(e => e.ID == item.AreaId && e.VersionId == item.VersionId && e.DesignBriefId == model.DesignBriefId);
                                if (araEntity == null)
                                {
                                    araEntity = context.DesignBriefPolicyRequirementAreas.SingleOrDefault(e => e.ID == item.AreaId && e.VersionId == item.VersionId && e.DesignBriefId == model.DesignBriefId);
                                    if (araEntity == null)
                                    {
                                        araEntity = new DesignBriefPolicyRequirementArea();
                                        araEntity.ID = item.Area.Id;
                                        araEntity.Description = item.Area.Description;
                                        araEntity.VersionId = item.Area.VersionId;
                                        araEntity.DesignBriefId = model.DesignBriefId;
                                        context.DesignBriefPolicyRequirementAreas.AddObject(araEntity);

                                        await context.SaveChangesAsync();
                                    }
                                }

                                DesignBriefPolicyRequirementSubject sbjEntity = context.LoadedEntities<DesignBriefPolicyRequirementSubject>().SingleOrDefault(e => e.ID == item.SubjectId && e.VersionId == item.VersionId && e.DesignBriefId == model.DesignBriefId);
                                if (sbjEntity == null)
                                {
                                    sbjEntity = context.DesignBriefPolicyRequirementSubjects.SingleOrDefault(e => e.ID == item.SubjectId && e.VersionId == item.VersionId && e.DesignBriefId == model.DesignBriefId);
                                    if (sbjEntity == null)
                                    {
                                        sbjEntity = new DesignBriefPolicyRequirementSubject();
                                        sbjEntity.ID = item.Subject.Id;
                                        sbjEntity.Description = item.Subject.Description;
                                        sbjEntity.VersionId = item.Subject.VersionId;
                                        sbjEntity.DesignBriefId = model.DesignBriefId;
                                        context.DesignBriefPolicyRequirementSubjects.AddObject(sbjEntity);

                                        await context.SaveChangesAsync();
                                    }
                                }

                                DesignBriefPolicyRequirementSubject childSubjectEntity = context.LoadedEntities<DesignBriefPolicyRequirementSubject>().SingleOrDefault(e => e.ID == item.ChildSubjectId && e.VersionId == item.VersionId && e.DesignBriefId == model.DesignBriefId);
                                if (childSubjectEntity == null)
                                {
                                    childSubjectEntity = context.DesignBriefPolicyRequirementSubjects.SingleOrDefault(e => e.ID == item.ChildSubjectId && e.VersionId == item.VersionId && e.DesignBriefId == model.DesignBriefId);
                                    if (childSubjectEntity == null)
                                    {
                                        if (item.ChildSubject.Id > 0)
                                        {
                                            childSubjectEntity = new DesignBriefPolicyRequirementSubject();
                                            childSubjectEntity.ID = item.ChildSubject.Id;
                                            childSubjectEntity.Description = item.ChildSubject.Description;
                                            childSubjectEntity.VersionId = item.ChildSubject.VersionId;
                                            childSubjectEntity.DesignBriefId = model.DesignBriefId;
                                            context.DesignBriefPolicyRequirementSubjects.AddObject(childSubjectEntity);

                                            await context.SaveChangesAsync();
                                        }
                                    }
                                }

                                ProcessSourceReferences(item, model.DesignBriefId, context);
                                ProcessAttachments(item, model.DesignBriefId, dbfEntity.Code, context);
                                ProcessStrictness(item, model.DesignBriefId, context);

                                await context.SaveChangesAsync();
                            }
                        }

                        scope.Complete();

                        result = 1;
                    }
                    catch (Exception error)
                    {
                        scope.Dispose();

                        throw error;
                    }
                }
            }

            return result;
        }

        public async Task<int> Delete(int designBriefId, int[] ids)
        {
            var result = 0;

            using (var context = NewContext())
            {
                using (TransactionScope mainScope = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.Parse("60"), TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            var assStrs = context.DesignBriefPolicyRequirementAssignedStrictnesses.Where(e => e.DesignBriefId == designBriefId && e.DesignBriefPolicyRequirementId == id).ToArray();
                            var assSrcEnts = context.DesignBriefPolicyRequirementAssignedSources.Where(e => e.DesignBriefId == designBriefId && e.DesignBriefPolicyRequirementId == id).ToArray();
                            var assAttEnts = context.DesignBriefPolicyRequirementAssignedAttachments.Where(e => e.DesignBriefId == designBriefId && e.DesignBriefPolicyRequirementId == id).ToArray();

                            foreach (var assEnt in assStrs)
                            {
                                context.DesignBriefPolicyRequirementAssignedStrictnesses.DeleteObject(assEnt);
                            }

                            foreach (var assEnt in assSrcEnts)
                            {
                                context.DesignBriefPolicyRequirementAssignedSources.DeleteObject(assEnt);
                            }

                            foreach (var assEnt in assAttEnts)
                            {
                                context.DesignBriefPolicyRequirementAssignedAttachments.DeleteObject(assEnt);
                            }

                            await context.SaveChangesAsync();

                            var topics = context.DesignBriefTopics.Where(e => e.DesignBriefId == designBriefId);
                            var entity = context.DesignBriefItems.SingleOrDefault(e => e.DesignBriefId == designBriefId && e.Id == id);

                            if (null != entity)
                            {
                                context.DesignBriefItems.DeleteObject(entity);
                            }

                            foreach (var ent in context.DesignBriefPolicyRequirementChapters.Where(e => e.DesignBriefId == designBriefId))
                            {
                                var ents = context.DesignBriefItems.Where(e => e.DesignBriefId == designBriefId && e.ChapterId == ent.ID);
                                if (ents.Count() == 0)
                                {
                                    context.DesignBriefPolicyRequirementChapters.DeleteObject(ent);
                                }
                            }

                            foreach (var ent in context.DesignBriefPolicyRequirementAreas.Where(e => e.DesignBriefId == designBriefId))
                            {
                                var ents = context.DesignBriefItems.Where(e => e.DesignBriefId == designBriefId && e.AreaId == ent.ID);
                                if (ents.Count() == 0)
                                {
                                    context.DesignBriefPolicyRequirementAreas.DeleteObject(ent);
                                }
                            }

                            foreach (var ent in context.DesignBriefPolicyRequirementSubjects.Where(e => e.DesignBriefId == designBriefId))
                            {
                                var ents = context.DesignBriefItems.Where(e => e.DesignBriefId == designBriefId && (e.SubjectId == ent.ID || e.ChildSubjectId == ent.ID));
                                if (ents.Count() == 0)
                                {
                                    context.DesignBriefPolicyRequirementSubjects.DeleteObject(ent);
                                }
                            }

                            foreach (var ent in context.DesignBriefPolicyRequirementSources.Where(e => e.DesignBriefId == designBriefId))
                            {
                                var assEnts = context.DesignBriefPolicyRequirementAssignedSources.Where(e => e.DesignBriefId == designBriefId && e.SourceId == ent.ID);
                                if (assEnts.Count() == 0)
                                {
                                    context.DesignBriefPolicyRequirementSources.DeleteObject(ent);
                                }
                            }

                            foreach (var ent in context.DesignBriefPolicyRequirementAttachments.Where(e => e.DesignBriefId == designBriefId))
                            {
                                var assEnts = context.DesignBriefPolicyRequirementAssignedAttachments.Where(e => e.DesignBriefId == designBriefId && e.AttachmentId == ent.ID);
                                if (assEnts.Count() == 0)
                                {
                                    context.DesignBriefPolicyRequirementAttachments.DeleteObject(ent);
                                }
                            }

                            await context.SaveChangesAsync();
                        }

                        mainScope.Complete();
                    }
                    catch (Exception error)
                    {
                        mainScope.Dispose();

                        throw error;
                    }
                }
            }

            result = 1;

            return result;
        }

        private DesignBriefAttachmentBaseModel MapToModel(DesignBriefPolicyRequirementAttachment entity)
        {
            DesignBriefAttachmentBaseModel model = new DesignBriefAttachmentBaseModel();

            if (entity != null)
            {
                model.Id = entity.ID;
                model.LiorId = entity.LiorID;
                model.Description = entity.Description;
                model.BaseLocation = entity.StorageLocation;
                model.FileName = entity.FileName;
                model.StorageLocation = (!string.IsNullOrEmpty(entity.StorageLocation) && entity.StorageLocation.Contains(_FileFolder)) ? entity.StorageLocation.Replace("~/", _BaseUrl) : entity.StorageLocation;
            }

            return model;
        }

        private static DesignBriefPolicyRequirementAttachment MapToEntity(DesignBriefAttachmentBaseModel model, int designBriefId, string designBriefCode)
        {
            DesignBriefPolicyRequirementAttachment entity = new DesignBriefPolicyRequirementAttachment();

            if (model != null)
            {
                entity.LiorID = model.Id;
                entity.DesignBriefId = designBriefId;
                entity.Description = model.Description;
                entity.StorageLocation = string.Format(BasePath, "~/", designBriefId.ToString() + "-" + designBriefCode.Replace(" ", "_"), model.FileName, "/");
                entity.FileName = model.FileName;
            }

            return entity;
        }

        private DesignBriefSourceReferenceModel MapToModel(DesignBriefPolicyRequirementSource entity)
        {
            DesignBriefSourceReferenceModel model = new DesignBriefSourceReferenceModel();

            if (entity != null)
            {
                model.Id = entity.ID;
                model.LiorId = entity.LiorID;
                model.Description = entity.Description;
                model.StorageLocation = entity.StorageLocation;
            }

            return model;
        }

        private DesignBriefSeverityModel MapToModel(DesignBriefPolicyRequirementStrictness entity)
        {
            DesignBriefSeverityModel model = new DesignBriefSeverityModel();

            if (entity != null)
            {
                model.Id = entity.ID;
                model.Description = entity.Description;
                model.ShortName = entity.ShortName;
                model.Clarification = entity.HelpText;
            }

            return model;
        }

        public async Task<DesignBriefAttachmentBaseModel> GetAttachment(int designBriefId, int id)
        {
            DesignBriefPolicyRequirementAttachment dbAttEntity = null;

            var entity = await _Context.DesignBriefPolicyRequirementAssignedAttachments.SingleOrDefaultAsync(e => e.DesignBriefId == designBriefId && e.AttachmentId == id);

            if (entity == null)
            {
                dbAttEntity = await _Context.DesignBriefPolicyRequirementAttachments.SingleOrDefaultAsync(e => e.DesignBriefId == designBriefId && e.ID == id);
            }
            else
            {
                dbAttEntity = entity.DesignBriefPolicyRequirementAttachments;
            }

            var model = MapToModel(dbAttEntity);

            return model;
        }

        public async Task<DesignBriefSourceReferenceModel> GetSourceReference(int designBriefId, int id)
        {
            DesignBriefPolicyRequirementSource dbSrcEntity = null;

            var entity = await _Context.DesignBriefPolicyRequirementAssignedSources.SingleOrDefaultAsync(e => e.DesignBriefId == designBriefId && e.SourceId == id);

            if (entity == null)
            {
                dbSrcEntity = await _Context.DesignBriefPolicyRequirementSources.SingleOrDefaultAsync(e => e.DesignBriefId == designBriefId && e.ID == id);
            }
            else
            {
                dbSrcEntity = entity.DesignBriefPolicyRequirementSource;
            }

            var model = MapToModel(dbSrcEntity);

            return model;
        }

        public async Task<DesignBriefSeverityModel> GetStrictness(int designBriefId, int id)
        {
            var entity = await _Context.DesignBriefPolicyRequirementAssignedStrictnesses.SingleOrDefaultAsync(e => e.DesignBriefId == designBriefId && e.StrictnessId == id);

            var model = MapToModel(entity.DesignBriefPolicyRequirementStrictness);

            return model;
        }

        public async Task<List<DesignBriefAttachmentBaseModel>> GetAssignedAttachments(int designBriefId, int designBriefPolicyRequirementId)
        {
            var models = new List<DesignBriefAttachmentBaseModel>();

            var assEntities = await _Context.DesignBriefPolicyRequirementAssignedAttachments.Where(e => e.DesignBriefId == designBriefId && e.DesignBriefPolicyRequirementId == designBriefPolicyRequirementId).ToListAsync();

            foreach (var entity in assEntities)
            {
                var model = MapToModel(entity.DesignBriefPolicyRequirementAttachments);

                model.PolicyRequirementId = entity.PolicyRequirementId;

                model.DesignBriefPolicyRequirementId = entity.DesignBriefPolicyRequirementId;

                models.Add(model);
            }

            return models;
        }

        public async Task<List<DesignBriefAttachmentBaseModel>> GetAssignedAttachments(int designBriefId)
        {
            var models = new List<DesignBriefAttachmentBaseModel>();

            var assEntities = await _Context.DesignBriefPolicyRequirementAssignedAttachments.Where(e => e.DesignBriefId == designBriefId).ToListAsync();

            foreach (var entity in assEntities)
            {
                if (entity.DesignBriefPolicyRequirementAttachments != null)
                {
                    var model = MapToModel(entity.DesignBriefPolicyRequirementAttachments);

                    model.PolicyRequirementId = entity.PolicyRequirementId;

                    model.DesignBriefPolicyRequirementId = entity.DesignBriefPolicyRequirementId;

                    models.Add(model);
                }
            }

            return models;
        }

        public async Task<List<DesignBriefAttachmentBaseModel>> GetAttachments(int designBriefId)
        {
            var assEntities = await _Context.DesignBriefPolicyRequirementAttachments.Where(e => e.DesignBriefId == designBriefId).ToListAsync();

            var models = assEntities.Select(e => MapToModel(e)).OrderBy(m => m.Description).ToList();

            return models;
        }

        public async Task<List<DesignBriefSourceReferenceModel>> GetSourceReferences(int designBriefId)
        {
            var assEntities = await _Context.DesignBriefPolicyRequirementSources.Where(e => e.DesignBriefId == designBriefId).ToArrayAsync();

            var models = assEntities.Select(e => MapToModel(e)).OrderBy(m => m.Description).ToList();

            return models;
        }

        public async Task<List<DesignBriefSourceReferenceModel>> GetAssignedSourceReferences(int designBriefId, int designBriefPolicyRequirementId)
        {
            var models = new List<DesignBriefSourceReferenceModel>();

            var assEntities = await _Context.DesignBriefPolicyRequirementAssignedSources.Where(e => e.DesignBriefId == designBriefId && e.DesignBriefPolicyRequirementId == designBriefPolicyRequirementId).ToArrayAsync();

            foreach (var entity in assEntities)
            {
                var model = MapToModel(entity.DesignBriefPolicyRequirementSource);
                model.PolicyRequirementId = entity.PolicyRequirementId;
                model.DesignBriefPolicyRequirementId = entity.DesignBriefPolicyRequirementId;
                models.Add(model);
            }

            return models;
        }

        public async Task<List<DesignBriefSourceReferenceModel>> GetAssignedSourceReferences(int designBriefId)
        {
            var models = new List<DesignBriefSourceReferenceModel>();

            var assEntities = await _Context.DesignBriefPolicyRequirementAssignedSources.Where(e => e.DesignBriefId == designBriefId).ToArrayAsync();

            foreach (var entity in assEntities)
            {
                var model = MapToModel(entity.DesignBriefPolicyRequirementSource);
                model.PolicyRequirementId = entity.PolicyRequirementId;
                model.DesignBriefPolicyRequirementId = entity.DesignBriefPolicyRequirementId;
                models.Add(model);
            }

            return models;
        }

        public async Task<List<DesignBriefSeverityModel>> GetAssignedStrictnessess(int designBriefId, int designBriefPolicyRequirementId)
        {
            var assEntities = await _Context.DesignBriefPolicyRequirementAssignedStrictnesses.Where(e => e.DesignBriefId == designBriefId && e.DesignBriefPolicyRequirementId == designBriefPolicyRequirementId).ToArrayAsync();

            var models = assEntities.Where(e => e.DesignBriefPolicyRequirementStrictness != null).Select(e => MapToModel(e.DesignBriefPolicyRequirementStrictness)).ToList();

            return models;
        }

        public async Task<List<DesignBriefSeverityModel>> GetAssignedStrictnessess(int designBriefId)
        {
            var assEntities = await _Context.DesignBriefPolicyRequirementAssignedStrictnesses.Where(e => e.DesignBriefId == designBriefId).ToListAsync();

            var models = assEntities.Where(e => e.DesignBriefPolicyRequirementStrictness != null).Select(e => MapToModel(e.DesignBriefPolicyRequirementStrictness)).ToList();

            return models;
        }

        private static void ProcessSourceReferences(DesignBriefPolicyRequirementModel item, int designBriefId, DesignBriefDataEntityModels context)
        {
            var entities = context.LoadedEntities<DesignBriefPolicyRequirementSource>();

            var assEntities = context.LoadedEntities<DesignBriefPolicyRequirementAssignedSource>();

            bool isNew = false;

            foreach (var src in item.SourceDocuments)
            {
                var entity = entities.SingleOrDefault(e => (e.ID == src.Id || e.LiorID == src.Id) && e.DesignBriefId == designBriefId);
                if (entity == null)
                {
                    entity = context.DesignBriefPolicyRequirementSources.SingleOrDefault(e => (e.ID == src.Id || e.LiorID == src.Id) && e.DesignBriefId == designBriefId);
                    if (entity == null)
                    {
                        entity = new DesignBriefPolicyRequirementSource();
                        entity.LiorID = src.Id;
                        entity.DesignBriefId = designBriefId;
                        entity.Description = src.Description;
                        entity.StorageLocation = src.StorageLocation;
                        context.DesignBriefPolicyRequirementSources.AddObject(entity);
                        isNew = true;
                    }
                }

                if (isNew)
                {
                    context.SaveChanges();
                }

                var assEnt = assEntities.SingleOrDefault(e => e.DesignBriefId == designBriefId && e.DesignBriefPolicyRequirementId == item.Id && e.SourceId == entity.ID);
                if (assEnt == null)
                {
                    assEnt = context.DesignBriefPolicyRequirementAssignedSources.SingleOrDefault(e => e.DesignBriefId == designBriefId && e.DesignBriefPolicyRequirementId == item.Id && e.SourceId == entity.ID);
                    if (assEnt == null)
                    {
                        assEnt = new DesignBriefPolicyRequirementAssignedSource();
                        assEnt.DesignBriefId = designBriefId;
                        assEnt.DesignBriefPolicyRequirementId = item.Id;
                        assEnt.PolicyRequirementId = item.PolicyRequirementId;
                        assEnt.SourceId = entity.ID;
                        assEnt.VersionId = item.VersionId;
                        context.DesignBriefPolicyRequirementAssignedSources.AddObject(assEnt);
                    }
                }
            }
        }

        private static void ProcessAttachments(DesignBriefPolicyRequirementModel item, int designBriefId, string designBriefCode, DesignBriefDataEntityModels context)
        {
            var entities = context.LoadedEntities<DesignBriefPolicyRequirementAttachment>();

            var assEntities = context.LoadedEntities<DesignBriefPolicyRequirementAssignedAttachment>();

            var isNew = false;

            TxFileManager fileMgr = new TxFileManager();
            var localPath = string.Empty;

            foreach (var attachment in item.Attachments)
            {
                var entity = entities.SingleOrDefault(e => (e.ID == attachment.Id || e.LiorID == attachment.Id) && e.DesignBriefId == designBriefId);
                if (entity == null)
                {
                    entity = context.DesignBriefPolicyRequirementAttachments.SingleOrDefault(e => (e.ID == attachment.Id || e.LiorID == attachment.Id) && e.DesignBriefId == designBriefId);
                    if (entity == null)
                    {
                        entity = MapToEntity(attachment, designBriefId, designBriefCode);
                        context.DesignBriefPolicyRequirementAttachments.AddObject(entity);
                        isNew = true;
                    }
                }

                if (isNew)
                {
                    context.SaveChanges();
                }

                CopyAttachments(ref localPath, fileMgr, designBriefId, designBriefCode, BasePath, attachment.FileName);

                var assEnt = assEntities.SingleOrDefault(e => e.DesignBriefId == designBriefId && e.DesignBriefPolicyRequirementId == item.Id && e.AttachmentId == entity.ID);
                if (assEnt == null)
                {
                    assEnt = context.DesignBriefPolicyRequirementAssignedAttachments.SingleOrDefault(e => e.DesignBriefId == designBriefId && e.DesignBriefPolicyRequirementId == item.Id && e.AttachmentId == entity.ID);
                    if (assEnt == null)
                    {
                        assEnt = new DesignBriefPolicyRequirementAssignedAttachment();
                        assEnt.DesignBriefId = designBriefId;
                        assEnt.DesignBriefPolicyRequirementId = item.Id;
                        assEnt.PolicyRequirementId = item.PolicyRequirementId;
                        assEnt.AttachmentId = entity.ID;
                        assEnt.VersionId = item.VersionId;
                        context.DesignBriefPolicyRequirementAssignedAttachments.AddObject(assEnt);
                    }
                }
            }
        }

        private static void ProcessStrictness(DesignBriefPolicyRequirementModel item, int designBriefId, DesignBriefDataEntityModels context)
        {
            var assEntities = context.LoadedEntities<DesignBriefPolicyRequirementAssignedStrictness>();

            foreach (var str in item.Strictnesses)
            {
                var assEnt = assEntities.SingleOrDefault(e => e.DesignBriefId == designBriefId && e.DesignBriefPolicyRequirementId == item.Id && e.StrictnessId == str.Id && e.VersionId == item.VersionId);
                if (assEnt == null)
                {
                    assEnt = context.DesignBriefPolicyRequirementAssignedStrictnesses.SingleOrDefault(e => e.DesignBriefId == designBriefId && e.DesignBriefPolicyRequirementId == item.Id && e.StrictnessId == str.Id && e.VersionId == item.VersionId);
                    if (assEnt == null)
                    {
                        assEnt = new DesignBriefPolicyRequirementAssignedStrictness();
                        assEnt.DesignBriefId = designBriefId;
                        assEnt.DesignBriefPolicyRequirementId = item.Id;
                        assEnt.PolicyRequirementId = item.PolicyRequirementId;
                        assEnt.StrictnessId = str.Id;
                        assEnt.VersionId = item.VersionId;
                        context.DesignBriefPolicyRequirementAssignedStrictnesses.AddObject(assEnt);
                    }
                }
            }
        }

        private static void CopyAttachments(ref string localPath, TxFileManager fileMgr, int designBriefId, string designBriefCode, string basePath, string fileName)
        {
            if (string.IsNullOrEmpty(localPath))
            {
                localPath = string.Format("{0}Files\\Attachments\\{1}-{2}", AppDomain.CurrentDomain.BaseDirectory, designBriefId, designBriefCode.Replace(" ", "_"));

                if (!fileMgr.DirectoryExists(localPath))
                    fileMgr.CreateDirectory(localPath);
            }

            var dbfFile = string.Format(basePath, AppDomain.CurrentDomain.BaseDirectory, designBriefId.ToString() + "-" + designBriefCode.Replace(" ", "_"), fileName, "\\");

            if (!fileMgr.FileExists(dbfFile))
            {
                var file = string.Format("{0}Files\\Attachments\\{1}", AppDomain.CurrentDomain.BaseDirectory, fileName);
                if (fileMgr.FileExists(file))
                {
                    fileMgr.Copy(file, dbfFile, true);
                }
            }
        }

        public void AssignStrictness(DesignBriefPolicyRequirementModel model, int severityId, int versionId)
        {
            var entity = new DesignBriefPolicyRequirementAssignedStrictness();
            entity.PolicyRequirementId = model.Id;
            entity.StrictnessId = severityId;
            entity.VersionId = versionId;
            _Context.DesignBriefPolicyRequirementAssignedStrictnesses.AddObject(entity);
        }

        public void AssignSourceDocument(DesignBriefPolicyRequirementModel model, int sourceId, int versionId)
        {
            var entity = new DesignBriefPolicyRequirementAssignedSource();
            entity.PolicyRequirementId = model.Id;
            entity.SourceId = sourceId;
            entity.VersionId = versionId;
            _Context.DesignBriefPolicyRequirementAssignedSources.AddObject(entity);
        }

        public void AssignAttachment(DesignBriefPolicyRequirementModel model, int attachmentId, int versionId)
        {
            var entity = new DesignBriefPolicyRequirementAssignedAttachment();
            entity.PolicyRequirementId = model.Id;
            entity.AttachmentId = attachmentId;
            entity.VersionId = versionId;
            _Context.DesignBriefPolicyRequirementAssignedAttachments.AddObject(entity);
        }

        public async Task<int> UpdateOrder(int designBriedId, int id, int orderIndex, string userName)
        {
            var result = 0;

            var entity = await _Context.DesignBriefItems.Where(e => e.DesignBriefId == designBriedId && e.Id == id).SingleOrDefaultAsync();

            if (entity != null)
            {
                var topic = await _Context.DesignBriefTopics.SingleOrDefaultAsync(e => e.DesignBriefId == designBriedId && e.LiorId == entity.ChapterId);

                if (topic != null)
                {
                    if (!topic.Owner.ToLower().Equals(userName.ToLower()))
                        return -1;

                    entity.OrderIndex = orderIndex;

                    result = 1;
                }
            }

            return result;
        }

        public async Task<int> AddAttachments(List<DesignBriefSimpleMediaModel> models)
        {
            int result = 0;

            using (var context = NewContext())
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.Parse("60"), TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        foreach (var model in models)
                        {
                            if (model.PolicyRequirementId > 0 && !string.IsNullOrEmpty(model.FileName) && !string.IsNullOrEmpty(model.LabelName))
                            {
                                var preqEntity = context.DesignBriefItems.SingleOrDefault(e => e.Id == model.PolicyRequirementId);

                                if (preqEntity != null)
                                {
                                    var fileName = Path.GetFileName(model.FileName).Replace(" ", "_");
                                    var entity = new DesignBriefPolicyRequirementAttachment();
                                    entity.DesignBriefId = model.DesignBriefId;
                                    entity.Description = model.LabelName;
                                    entity.StorageLocation = string.Format(BasePath, "~/", model.DesignBriefId.ToString() + "-" + model.ProjectCode.Replace(" ", "_"), fileName, "/");
                                    entity.FileName = fileName;

                                    var assEnt = new DesignBriefPolicyRequirementAssignedAttachment();
                                    assEnt.DesignBriefId = model.DesignBriefId;
                                    assEnt.DesignBriefPolicyRequirementId = model.PolicyRequirementId;
                                    assEnt.AttachmentId = entity.ID;
                                    assEnt.VersionId = preqEntity.VersionId;

                                    context.DesignBriefPolicyRequirementAttachments.AddObject(entity);
                                    context.DesignBriefPolicyRequirementAssignedAttachments.AddObject(assEnt);

                                    await context.SaveChangesAsync();
                                }
                                else
                                    return -1;
                            }
                        }

                        await context.SaveChangesAsync();

                        scope.Complete();
                    }
                    catch (Exception error)
                    {
                        scope.Dispose();

                        throw error;
                    }
                }

                result = 1;
            }

            return result;
        }

        public async Task<int> DeassignAttachments(int[] ids, int designbriefId, int policyeRequirementId)
        {
            var result = -1;

            using (var context = NewContext())
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.Parse("60"), TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            var assigned = context.DesignBriefPolicyRequirementAssignedAttachments.SingleOrDefault(e => e.AttachmentId == id && e.DesignBriefId == designbriefId && e.DesignBriefPolicyRequirementId == policyeRequirementId);
                            if (assigned != null)
                            {
                                context.DesignBriefPolicyRequirementAssignedAttachments.DeleteObject(assigned);
                            }
                        }

                        result = await context.SaveChangesAsync();

                        scope.Complete();

                        result = 1;
                    }
                    catch (Exception error)
                    {
                        scope.Dispose();

                        throw error;
                    }
                }
            }

            return result;
        }

        public async Task<int> DeassignSourceReferences(int[] ids, int designbriefId, int policyeRequirementId)
        {
            var result = -1;

            using (var context = NewContext())
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.Parse("60"), TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            var assigned = context.DesignBriefPolicyRequirementAssignedSources.SingleOrDefault(e => e.SourceId == id && e.DesignBriefId == designbriefId && e.DesignBriefPolicyRequirementId == policyeRequirementId);
                            if (assigned != null)
                            {
                                context.DesignBriefPolicyRequirementAssignedSources.DeleteObject(assigned);
                            }
                        }

                        result = await context.SaveChangesAsync();

                        scope.Complete();

                        result = 1;
                    }
                    catch (Exception error)
                    {
                        scope.Dispose();

                        throw error;
                    }
                }
            }

            return result;
        }
    }
}
