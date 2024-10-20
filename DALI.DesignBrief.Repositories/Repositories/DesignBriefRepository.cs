using ChinhDo.Transactions;
using ChinhDo.Transactions.FileManager;
using DALI.DesignBrief.BusinessTypes.Enums;
using DALI.DesignBrief.DataEntityModels;
using DALI.DesignBrief.DomainModels;
using DALI.DesignBrief.DomainModels.Models;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace DALI.DesignBrief.Repositories
{
    public delegate DesignBriefManagerModel GetManagerInfoDelegateForRepository(string userName);

    public class DesignBriefRepository : Repository
    {
        public DesignBriefRepository(IDesignBriefDataEntityModelContext context) : base(context)
        {

        }

        private DataEntityModels.DesignBrief MapToEntity(DesignBriefModel model)
        {
            DataEntityModels.DesignBrief entity = new DataEntityModels.DesignBrief();

            entity.Id = model.Id;
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.StatusId = model.Status.Id;
            entity.Manager = model.Manager;
            entity.Code = model.Code;
            entity.BaseDesignBriefId = model.BaseId;

            return entity;
        }

        private DesignBriefModel MapToModel(DataEntityModels.DesignBrief entity)
        {
            DesignBriefModel model = new DesignBriefModel();

            // Fill model properties
            model.Id = entity.Id;
            model.Name = entity.Name;
            model.Status.Id = entity.StatusId;
            model.Manager = entity.Manager;
            model.Code = entity.Code;
            model.Description = entity.Description;
            model.BaseId = entity.BaseDesignBriefId;
            model.Comment = entity.Comment;

            return model;
        }

        private void FillDetails(DesignBriefModel model, DataEntityModels.DesignBrief entity)
        {
            model.Agreement.Id = entity.DesignBriefAgreement.Id;
            model.Agreement.Description = entity.DesignBriefAgreement.Name;
            model.Status.Id = entity.StatusId;
            model.Version = entity.DesignBriefVersion ?? 1;
            model.Comment = entity.Comment;

            if (entity.StatusId > 0)
            {
                model.Status.Name = entity.DesignBriefStatu.Name;
            }

            model.Manager = entity.Manager;
            model.CreatedBy = entity.CreatedBy;
            model.CreatedDate = entity.CreatedDate ?? default(DateTime);
            model.ModifiedBy = entity.ModifiedBy;
            model.ModifiedDate = entity.ModifiedDate ?? default(DateTime);


            IEnumerable<DesignBriefTopicModel> topics = entity.DesignBriefTopic.Select(topic => new DesignBriefTopicModel()
            {
                DesignBriefId = topic.DesignBriefId,
                Id = topic.Id,
                LiorId = topic.LiorId ?? default(int),
                VersionId = topic.VersionId,
                Description = topic.Description,
                Owner = topic.Owner,
                InUse = topic.InUse ?? default(bool),
                Imported = topic.Imported ?? default(bool),
                OwnerEmailAddress = topic.OwnerEmailAddress,
                ChapterNumber = topic.ChapterNumber
            });


            IEnumerable<DesignBriefTypeModel> types = entity.DesignBriefTypeReference.Select(e => new DesignBriefTypeModel()
            {
                Id = e.DesignBriefTypes.Id,
                Description = e.DesignBriefTypes.Name
            });

            // the default projectmembers
            IEnumerable<DesignBriefMemberModel> projectMembers = entity.DesignBriefMember.Where(e => e.MemberBySubscription == false).Select(e => new DesignBriefMemberModel()
            {
                DesignBriefId = e.DesignBriefId,
                Editor = e.Editor ?? default(bool),
                OfficeInfo = e.OfficeInfo,
                UserName = e.UserName,
                MemberBySubscription = false
            });


            // (external) added by subscription
            var subscribedMembers = entity.DesignBriefMember.Where(e => e.MemberBySubscription == true).Select(e => new DesignBriefMemberModel()
            {
                DesignBriefId = e.DesignBriefId,
                Editor = e.Editor ?? default(bool),
                OfficeInfo = e.OfficeInfo,
                UserName = e.UserName,
                MemberBySubscription = true
            });


            model.Topics.AddRange(topics);
            model.Types.AddRange(types);
            model.Members.AddRange(projectMembers);
            model.MembersBySubscription.AddRange(subscribedMembers);

            DesignBriefMemberModel createdByMember = model.Members.SingleOrDefault(m => string.Compare(m.UserName, model.CreatedBy, true) == 0);
            if (createdByMember == null)
            {
                var memberModel = new DesignBriefMemberModel();

                memberModel.DesignBriefId = model.Id;
                memberModel.Editor = true;
                memberModel.OfficeInfo = string.Empty;
                memberModel.UserName = model.CreatedBy;

                model.Members.Add(memberModel);
            }
        }

        private async Task<DataEntityModels.DesignBrief> GetOneEntityAsync(int id)
        {
            var entity = await _Context.DesignBriefs.SingleOrDefaultAsync(e => e.Id == id);

            return entity;
        }

        private IQueryable<DataEntityModels.DesignBrief> GetEntities(string userName)
        {
            IQueryable<int> ids = _Context.DesignBriefMembers.AsNoTracking().Where(e => e.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)).Select(e => e.DesignBriefId);

            IQueryable<DataEntityModels.DesignBrief> entities = _Context.DesignBriefs.AsNoTracking().Where(e => ids.Contains(e.Id) || string.Compare(e.Manager, userName, true) == 0 || e.CreatedBy.Equals(userName, StringComparison.OrdinalIgnoreCase) || e.ModifiedBy.Equals(userName, StringComparison.OrdinalIgnoreCase));

            return entities;
        }


        private IQueryable<DataEntityModels.DesignBrief> GetEntities()
        {
            IQueryable<int> ids = _Context.DesignBriefMembers.AsNoTracking().Select(e => e.DesignBriefId);

            IQueryable<DataEntityModels.DesignBrief> entities = _Context.DesignBriefs.AsNoTracking().Where(e => ids.Contains(e.Id));

            return entities;
        }



        private DesignBriefModel TransferDetailsToModel(DataEntityModels.DesignBrief entity)
        {
            DesignBriefModel model = MapToModel(entity);

            FillDetails(model, entity);

            return model;
        }

        public async Task<int> GetPublicationVersion(int designBriefId)
        {
            var entity = await _Context.DesignBriefItems.Where(e => e.DesignBriefId == designBriefId).OrderByDescending(u => u.VersionId).FirstOrDefaultAsync();

            if (entity == null)
                return -1;

            return entity.VersionId;
        }

        public async Task<DesignBriefModel[]> GetAll(string userName)
        {
            DataEntityModels.DesignBrief[] entities = await GetEntities(userName).OrderByDescending(e => e.CreatedDate)
                    .OrderBy(e => e.Description).ToArrayAsync();

            DesignBriefModel[] models = entities
                    .Select(e => TransferDetailsToModel(e))
                    .ToArray();

            return models;
        }

        public async Task<DesignBriefModel[]> GetAll()
        {
            DataEntityModels.DesignBrief[] entities = await GetEntities().ToArrayAsync();

            DesignBriefModel[] models = entities.OrderByDescending(e => e.CreatedDate)
                    .OrderBy(e => e.Description)
                    .Select(e => TransferDetailsToModel(e))
                    .ToArray();

            return models;
        }

        public async Task<DesignBriefModel[]> GetCurrents(string userName)
        {
            DesignBriefModel[] models = await GetAll(userName);

            DesignBriefModel[] result = models.Where(e => DesignBriefStatusEnumExt.AsEnum(e.Status.Id) == DesignBriefStatusEnum.Concept || DesignBriefStatusEnumExt.AsEnum(e.Status.Id) == DesignBriefStatusEnum.Secured).OrderByDescending(e => e.ModifiedDate).OrderBy(e => e.Description).ToArray();

            return result;
        }

        public async Task<DesignBriefModel[]> GetDispatched(string userName)
        {
            var models = await GetAll(userName);

            var result = models.Where(e => DesignBriefStatusEnumExt.AsEnum(e.Status.Id) == DesignBriefStatusEnum.Canceled || DesignBriefStatusEnumExt.AsEnum(e.Status.Id) == DesignBriefStatusEnum.Finished).OrderByDescending(e => e.ModifiedDate).OrderBy(e => e.Description).ToArray();

            return result;
        }

        public async Task<DesignBriefModel> GetOneModelAsync(int id)
        {
            var entity = await GetOneEntityAsync(id);

            var model = TransferDetailsToModel(entity);

            return model;
        }

        public async Task<int> GetCurrentId(string prjCode)
        {
            var entity = await _Context.DesignBriefs.AsNoTracking().SingleOrDefaultAsync(e => e.Code == prjCode);

            return entity != null ? entity.Id : -1;
        }

        public async Task<int> Update(DesignBriefModel model, string userName)
        {
            int result = 0;

            //var context = _Context;

            using (var context = NewContext())
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.Parse("60"), TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        var entity = await context.DesignBriefs.SingleOrDefaultAsync(e => e.Id == model.Id);
                        if (entity != null)
                        {
                            var typeRefs = context.DesignBriefTypeReferences.Where(e => e.DesignBriefId == entity.Id);
                            var members = context.DesignBriefMembers.Where(e => e.DesignBriefId == entity.Id);

                            foreach (var typeRef in typeRefs)
                            {
                                context.DesignBriefTypeReferences.DeleteObject(typeRef);
                            }

                            foreach (var member in members)
                            {
                                context.DesignBriefMembers.DeleteObject(member);
                            }

                            context.SaveChanges();

                            entity.Name = model.Name;
                            entity.Code = model.Code;
                            entity.ModifiedBy = userName;
                            entity.ModifiedDate = DateTime.Now;
                            entity.Description = model.Description;
                            entity.Manager = model.Manager;

                            if (model.Agreement.Id > 0)
                            {
                                entity.AgreementId = model.Agreement.Id;
                            }

                            foreach (var topicModel in model.Topics)
                            {
                                if (topicModel.InUse)
                                {
                                    var topic = context.DesignBriefTopics.SingleOrDefault(e => e.DesignBriefId == entity.Id && e.LiorId == topicModel.LiorId);
                                    if (topic != null)
                                    {
                                        topic.InUse = topicModel.InUse;
                                        topic.Owner = topicModel.Owner;
                                        topic.OwnerEmailAddress = topicModel.OwnerEmailAddress;
                                    }
                                }
                            }

                            foreach (var typeModel in model.Types)
                            {
                                var type = new DesignBriefTypeReference();

                                type.DesignBriefId = entity.Id;
                                type.DesignBriefTypeId = typeModel.Id;

                                context.DesignBriefTypeReferences.AddObject(type);
                            }

                            foreach (var memberModel in model.Members)
                            {
                                var member = new DesignBriefMember();

                                member.DesignBriefId = entity.Id;
                                member.Editor = memberModel.Editor;
                                member.OfficeInfo = memberModel.OfficeInfo;
                                member.UserName = memberModel.UserName;
                                member.MemberBySubscription = false;

                                context.DesignBriefMembers.AddObject(member);
                            }

                            foreach (var memberModel in model.MembersBySubscription)
                            {
                                var member = new DesignBriefMember();

                                member.DesignBriefId = entity.Id;
                                member.Editor = memberModel.Editor;
                                member.OfficeInfo = memberModel.OfficeInfo;
                                member.UserName = memberModel.UserName;
                                member.MemberBySubscription = true;

                                context.DesignBriefMembers.AddObject(member);
                            }

                            context.SaveChanges();

                            result = 1;
                        }

                        scope.Complete();
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

        public async Task<int> Update(int id, int status, string userName, string comment)
        {
            int result = 0;

            var entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                entity.ModifiedBy = userName;
                entity.ModifiedDate = DateTime.Now;
                entity.StatusId = status;
                entity.Comment = comment;

                result = 1;
            }

            return result;
        }

        public async Task<int> Update(int id, int status, string userName)
        {
            int result = 0;

            var entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                entity.ModifiedBy = userName;
                entity.ModifiedDate = DateTime.Now;
                entity.StatusId = status;


                result = 1;
            }

            return result;
        }

        public async Task<int> Delete(int id, string userName)
        {
            int result = 0;

            var context = _Context;

            DataEntityModels.DesignBrief entity = await context.DesignBriefs.SingleOrDefaultAsync(e => e.Id == id);

            int? baseId = null;
            if (entity != null)
            {
                if ((DesignBriefStatusEnumExt.AsEnum(entity.StatusId) == DesignBriefStatusEnum.Concept || DesignBriefStatusEnumExt.AsEnum(entity.StatusId) == DesignBriefStatusEnum.Canceled) && string.Compare(entity.Manager, userName, true) == 0 || string.Compare(entity.CreatedBy, userName, true) == 0)
                {
                    baseId = entity.BaseDesignBriefId;

                    await context.DesignBriefItems.Where(e => e.DesignBriefId == id).ForEachAsync(e => context.DesignBriefItems.DeleteObject(e));
                    await context.DesignBriefPolicyRequirementAssignedStrictnesses.Where(e => e.DesignBriefId == id).ForEachAsync(e => context.DesignBriefPolicyRequirementAssignedStrictnesses.DeleteObject(e));
                    await context.DesignBriefPolicyRequirementAssignedAttachments.Where(e => e.DesignBriefId == id).ForEachAsync(e => context.DesignBriefPolicyRequirementAssignedAttachments.DeleteObject(e));
                    await context.DesignBriefPolicyRequirementAssignedSources.Where(e => e.DesignBriefId == id).ForEachAsync(e => context.DesignBriefPolicyRequirementAssignedSources.DeleteObject(e));
                    await context.DesignBriefTypeReferences.Where(e => e.DesignBriefId == id).ForEachAsync(e => context.DesignBriefTypeReferences.DeleteObject(e));
                    await context.DesignBriefPolicyRequirementAttachments.Where(e => e.DesignBriefId == id).ForEachAsync(e => context.DesignBriefPolicyRequirementAttachments.DeleteObject(e));
                    await context.DesignBriefTopics.Where(e => e.DesignBriefId == id).ForEachAsync(e => context.DesignBriefTopics.DeleteObject(e));
                    await context.DesignBriefMembers.Where(e => e.DesignBriefId == id).ForEachAsync(e => context.DesignBriefMembers.DeleteObject(e));
                    await context.DesignBriefAttachments.Where(e => e.DesignBriefId == id).ForEachAsync(e => context.DesignBriefAttachments.DeleteObject(e));
                    await context.DesignBriefPolicyRequirementChapters.Where(e => e.DesignBriefId == id).ForEachAsync(e => context.DesignBriefPolicyRequirementChapters.DeleteObject(e));
                    await context.DesignBriefPolicyRequirementAreas.Where(e => e.DesignBriefId == id).ForEachAsync(e => context.DesignBriefPolicyRequirementAreas.DeleteObject(e));
                    await context.DesignBriefPolicyRequirementSubjects.Where(e => e.DesignBriefId == id).ForEachAsync(e => context.DesignBriefPolicyRequirementSubjects.DeleteObject(e));

                    context.DesignBriefs.DeleteObject(entity);
                }
                else
                {
                    var msg = string.Format(Resources.Localization.DeleteNotPermitted, entity.Name);
                    throw new Exception(msg);
                }
            }
            else
            {
                var msg = string.Format(Resources.Localization.DeleteNotPermitted, entity.Name);
                throw new Exception(msg);
            }


            using (TransactionScope tscope = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.Parse("60"), TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    context.SaveChanges();

                    TxFileManager fileMgr = new TxFileManager();
                    string localPath = string.Format("{0}Files\\Attachments\\{1}-{2}", AppDomain.CurrentDomain.BaseDirectory, id, entity.Code);

                    if (fileMgr.DirectoryExists(localPath))
                        fileMgr.DeleteDirectory(localPath);


                    if (baseId.HasValue)
                    {
                        var prevRevision = context.DesignBriefs.Where(e => e.BaseDesignBriefId == baseId || e.BaseDesignBriefId == null).OrderByDescending(e => e.CreatedDate).FirstOrDefault();

                        if (prevRevision != null)
                        {
                            prevRevision.StatusId = DesignBriefStatusEnum.Secured.AsInt();
                        }
                    }

                    context.SaveChanges();
                    tscope.Complete();

                    result = 1;
                }
                catch (Exception exc)
                {
                    tscope.Dispose();
                    throw exc;
                }

            }

            return result;
        }


        private async Task<DataEntityModels.DesignBrief> GetByNameOrCode(DesignBriefModel model)
        {
            var entities = await _Context.DesignBriefs.AsNoTracking().ToArrayAsync();

            var entity = entities.Where(e => string.Compare(e.Name, model.Name, true) == 0).OrderByDescending(e => e.DesignBriefVersion).FirstOrDefault();

            if (entity == null)
            {
                entity = entities.Where(e => string.Compare(e.Code, model.Code, true) == 0).OrderByDescending(e => e.DesignBriefVersion).FirstOrDefault();
            }

            return entity;
        }

        public async Task<int> Add(DesignBriefModel model, GetManagerInfoDelegateForRepository getManagerInfo)
        {
            int result = 0;

            if (getManagerInfo != null)
            {
                DataEntityModels.DesignBrief dbEntity = await GetByNameOrCode(model);

                if (dbEntity != null)
                {
                    var prjManagerExistingDB = getManagerInfo(dbEntity.Manager);

                    string managerInfo = string.Format("{0} - {1}", prjManagerExistingDB.FullName, prjManagerExistingDB.Email);

                    throw new Exception(string.Format(Resources.Localization.DesignBriefExists, managerInfo));
                }
            }

            //var context = _Context;
            using (var context = NewContext())
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        DataEntityModels.DesignBrief entity = MapToEntity(model);

                        entity.AgreementId = (model.Agreement.Id > 0) ? model.Agreement.Id : DesignBriefAgreementEnum.Unknown.AsInt();

                        entity.StatusId = 1;

                        entity.CreatedBy = model.CreatedBy;
                        entity.CreatedDate = model.CreatedDate;
                        entity.ModifiedBy = model.ModifiedBy;
                        entity.ModifiedDate = model.ModifiedDate;
                        entity.DesignBriefVersion = model.Version;

                        context.DesignBriefs.AddObject(entity);
                        context.SaveChanges();

                        model.Id = entity.Id;

                        foreach (DesignBriefTopicModel topicModel in model.Topics)
                        {
                            DesignBriefTopic topic = new DesignBriefTopic
                            {
                                DesignBriefId = entity.Id,
                                LiorId = topicModel.LiorId,
                                VersionId = topicModel.VersionId,
                                Description = topicModel.Description,
                                Owner = topicModel.Owner,
                                InUse = topicModel.InUse,
                                ChapterNumber = topicModel.ChapterNumber,
                                OwnerEmailAddress = topicModel.OwnerEmailAddress
                            };

                            entity.DesignBriefTopic.Add(topic);

                            context.SaveChanges();

                            topicModel.Id = topic.Id;
                        }

                        foreach (DesignBriefTypeModel typeModel in model.Types)
                        {
                            DesignBriefTypeReference type = new DesignBriefTypeReference();

                            type.DesignBriefId = entity.Id;
                            type.DesignBriefTypeId = typeModel.Id;

                            entity.DesignBriefTypeReference.Add(type);

                            context.SaveChanges();
                        }

                        // DALI V4, at this moment no subscribedMembers are transfered
                        // The projectmanager has to reassign the external users again
                        foreach (DesignBriefMemberModel memberModel in model.Members)
                        {
                            DesignBriefMember member = new DesignBriefMember
                            {
                                DesignBriefId = entity.Id,
                                Editor = memberModel.Editor,
                                OfficeInfo = memberModel.OfficeInfo,
                                UserName = memberModel.UserName,
                                MemberBySubscription = false
                            };

                            entity.DesignBriefMember.Add(member);

                            context.SaveChanges();
                        }

                        scope.Complete();

                        result = 1;
                    }
                    catch (Exception error)
                    {
                        scope.Dispose();
                        throw error;
                    }
                } // end transaction scope
            }

            return result;
        }

        public async Task AddMemberSubscriptionAsync(DesignBriefMemberModel model)
        {
            DesignBriefMember entity = new DesignBriefMember
            {
                DesignBriefId = model.DesignBriefId,
                Editor = model.Editor,
                OfficeInfo = model.OfficeInfo,
                UserName = model.UserName,
                MemberBySubscription = true
            };

            _Context.DesignBriefMembers.AddObject(entity);

            await _Context.SaveChangesAsync();
        }

        public async Task UnsubscribeMemberAsync(DesignBriefMemberModel model)
        {
            var entity = await _Context.DesignBriefMembers.AsNoTracking().SingleOrDefaultAsync(e => string.Compare(e.UserName, model.UserName, true) == 0 && e.DesignBriefId == model.DesignBriefId && e.MemberBySubscription == true);

            _Context.DesignBriefMembers.DeleteObject(entity);

            await _Context.SaveChangesAsync();
        }

        private async Task<DesignBriefMember[]> GetMemberEntitiesAsync()
        {
            DesignBriefMember[] entities = await _Context.DesignBriefMembers.AsNoTracking().ToArrayAsync();

            return entities;
        }

        public async Task<DesignBriefMemberModel[]> GetMembersAsync(int designBriefId)
        {
            DesignBriefMemberModel[] models = await _Context.DesignBriefMembers.AsNoTracking().Where(e => e.DesignBriefId == designBriefId).Select(e => new DesignBriefMemberModel()
            {
                DesignBriefId = e.DesignBriefId,
                UserName = e.UserName,
                Editor = (bool)e.Editor,
                OfficeInfo = e.OfficeInfo,
                MemberBySubscription = e.MemberBySubscription == true
            }).ToArrayAsync();

            return models;
        }

        public async Task<bool> HasMemberSubscription(string userName, int designBriefId)
        {
            DesignBriefMember[] entities = await GetMemberEntitiesAsync();

            DesignBriefMember entity = entities.SingleOrDefault(e => e.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) && e.DesignBriefId == designBriefId && e.MemberBySubscription == true);

            return entity != null;
        }

        public async Task<bool> HasSubscriptions(string userName)
        {
            DesignBriefMember[] entities = await GetMemberEntitiesAsync();

            IEnumerable<DesignBriefMember> subscriptions = entities.Where(e => e.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));

            return subscriptions.Count() > 0;
        }

        public async Task<DesignBriefMemberModel[]> GetAllMembersAsync()
        {
            DesignBriefMember[] entities = await GetMemberEntitiesAsync();

            var grouped = entities.GroupBy(e => e.UserName);

            // We only matter about an unique list of members, so the designbriefId does not matter to be set
            DesignBriefMemberModel[] models = grouped.Select(e => new DesignBriefMemberModel()
            {
                UserName = e.First().UserName,
                OfficeInfo = e.First().OfficeInfo,
            }).ToArray();

            return models;
        }

        public async Task<DesignBriefTopicModel[]> GetTopics(string owner, int designBriefId)
        {
            List<DesignBriefTopicModel> models = new List<DesignBriefTopicModel>();
            var topicEntities = await _Context.DesignBriefTopics.AsNoTracking().Where(e => e.DesignBriefId == designBriefId && e.Owner.Equals(owner, StringComparison.OrdinalIgnoreCase) && e.InUse == true).ToArrayAsync();

            foreach (var topic in topicEntities)
            {
                DesignBriefTopicModel topicModel = new DesignBriefTopicModel
                {
                    DesignBriefId = topic.DesignBriefId,
                    Id = topic.Id,
                    LiorId = topic.LiorId ?? default(int),
                    VersionId = topic.VersionId,
                    Description = topic.Description,
                    Owner = topic.Owner,
                    InUse = topic.InUse ?? default(bool),
                    Imported = topic.Imported ?? default(bool),
                    ChapterNumber = topic.ChapterNumber,
                    OwnerEmailAddress = topic.OwnerEmailAddress
                };

                models.Add(topicModel);
            }

            return models.OrderBy(m => m.FullDescription).ToArray();
        }

        public async Task<DesignBriefTopicModel> GetTopic(string owner, int designBriefId, int topicId)
        {
            DesignBriefTopicModel[] models = await GetTopics(owner, designBriefId);

            DesignBriefTopicModel model = models.SingleOrDefault(e => e.LiorId == topicId);

            return model;
        }

        public async Task<int> UpdateImportStatus(int designBriefId, int liorId, bool succeeded)
        {
            int result = 0;
            using (DesignBriefDataEntityModels context = NewContext())
            {
                DesignBriefTopic entity = await context.DesignBriefTopics.AsNoTracking().SingleOrDefaultAsync(e => e.DesignBriefId == designBriefId && e.LiorId == liorId);

                if (entity != null)
                {
                    entity.Imported = succeeded;
                    await context.SaveChangesAsync();
                    result = 1;
                }
            }
            return result;
        }

        public async Task<int> GetVersionNumber(int designBriefId, string designBriefCode)
        {
            var result = 1;

            using (DesignBriefDataEntityModels context = NewContext())
            {
                var latest = await context.DesignBriefs.AsNoTracking().Where(e => e.Code.Equals(designBriefCode, StringComparison.OrdinalIgnoreCase)).OrderByDescending(e => e.DesignBriefVersion).FirstOrDefaultAsync();

                int newVersion = latest != null && latest.DesignBriefVersion.HasValue ? latest.DesignBriefVersion.Value + 1 : 1;

                result = newVersion;
            }

            return result;
        }

        public async Task<IEnumerable<string>> GetProjectManagers()
        {
            IEnumerable<string> projectManagers = _Context.DesignBriefs.AsNoTracking().Select(e => e.Manager).Distinct();

            return projectManagers;
        }

        public async Task<string> GetProjectManager(int designBriefId)
        {
            DataEntityModels.DesignBrief entity = await _Context.DesignBriefs.AsNoTracking().SingleOrDefaultAsync(e => e.Id == designBriefId);

            return entity != null ? entity.Manager : string.Empty;
        }

        public async Task<DesignBriefMemberDetailsModel> GetMemberDetails(string userName)
        {
            IEnumerable<DesignBriefMember> entities = await _Context.DesignBriefMembers.AsNoTracking().Where(e => e.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)).ToArrayAsync();

            var designBriefIds = entities.Select(e => e.DesignBriefId).Distinct();

            if (designBriefIds.Any())
            {
                List<DesignBriefMemberDetailModel> designBriefDetailModels = new List<DesignBriefMemberDetailModel>();

                IEnumerable<DataEntityModels.DesignBrief> dbEntities = await _Context.DesignBriefs.AsNoTracking().Where(e => designBriefIds.Contains(e.Id)).ToArrayAsync();

                foreach (DataEntityModels.DesignBrief designBriefEntity in dbEntities)
                {
                    string Number = designBriefEntity.BaseDesignBriefId.HasValue ? string.Format("{0}-{1}", designBriefEntity.BaseDesignBriefId, designBriefEntity.DesignBriefVersion) : string.Format("{0}-{1}", designBriefEntity.Id, designBriefEntity.DesignBriefVersion);

                    DesignBriefMemberDetailModel designBriefDetailModel = new DesignBriefMemberDetailModel(designBriefEntity.Id, designBriefEntity.Code, designBriefEntity.Name, designBriefEntity.Description, Number, designBriefEntity.DesignBriefVersion, designBriefEntity.Manager);

                    designBriefDetailModels.Add(designBriefDetailModel);
                }

                DesignBriefMemberDetailsModel model = new DesignBriefMemberDetailsModel(userName, designBriefDetailModels.ToArray());

                return model;
            }

            return new DesignBriefMemberDetailsModel(userName, new DesignBriefMemberDetailModel[] { });
        }
    }
}
