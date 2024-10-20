using DALI.DesignBrief.DataEntityModels;
using DALI.DesignBrief.DomainModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace DALI.DesignBrief.Repositories
{
    public class DesignBriefSourceReferenceRepository : Repository
    {
        public DesignBriefSourceReferenceRepository(IDesignBriefDataEntityModelContext context) : base(context)
        {

        }

        private DesignBriefSourceReferenceModel MapToModel(DesignBriefPolicyRequirementSource entity)
        {
            DesignBriefSourceReferenceModel model = new DesignBriefSourceReferenceModel
            {
                Id = entity.ID,
                LiorId = entity.LiorID,
                Description = entity.Description,
                StorageLocation = entity.StorageLocation
            };

            return model;
        }

        private DesignBriefPolicyRequirementSource MapToEntity(DesignBriefSourceReferenceModel model)
        {
            DesignBriefPolicyRequirementSource entity = new DesignBriefPolicyRequirementSource
            {
                ID = model.Id,
                LiorID = model.LiorId,
                Description = model.Description,
                StorageLocation = model.StorageLocation
            };

            return entity;
        }

        public async Task<DesignBriefSourceReferenceModel[]> GetAll(int id)
        {
            DesignBriefPolicyRequirementSource[] entities = await _Context.DesignBriefPolicyRequirementSources.Where(e => e.DesignBriefId == id).ToArrayAsync();

            DesignBriefSourceReferenceModel[] models = entities.Select(e => MapToModel(e)).OrderBy(e => e.Description).ToArray();

            return models;
        }

        private async Task<DesignBriefPolicyRequirementSource> GetOneEntityAsync(int id)
        {
            var entity = await _Context.DesignBriefPolicyRequirementSources.SingleOrDefaultAsync(e => e.DesignBriefId == id);

            return entity;
        }

        public async Task<int> Add(List<DesignBriefSimpleMediaModel> models)
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
                            if (model.PolicyRequirementId > 0 && !string.IsNullOrEmpty(model.Url) && !string.IsNullOrEmpty(model.LabelName))
                            {
                                var preqEntity = context.DesignBriefItems.SingleOrDefault(e => e.Id == model.PolicyRequirementId);

                                if (preqEntity != null)
                                {
                                    DesignBriefPolicyRequirementSource entity = new DesignBriefPolicyRequirementSource
                                    {
                                        DesignBriefId = model.DesignBriefId,
                                        StorageLocation = model.Url,
                                        Description = model.LabelName
                                    };

                                    DesignBriefPolicyRequirementAssignedSource assEnt = new DesignBriefPolicyRequirementAssignedSource
                                    {
                                        DesignBriefId = model.DesignBriefId,
                                        DesignBriefPolicyRequirementId = model.PolicyRequirementId,
                                        SourceId = entity.ID,
                                        VersionId = preqEntity.VersionId
                                    };

                                    context.DesignBriefPolicyRequirementSources.AddObject(entity);
                                    context.DesignBriefPolicyRequirementAssignedSources.AddObject(assEnt);
                                    await context.SaveChangesAsync();
                                }
                                else
                                    return -1;
                            }
                        }

                        await context.SaveChangesAsync();

                        scope.Complete();

                        result = 1;
                    }
                    catch (Exception exc)
                    {
                        scope.Dispose();
                        throw exc;
                    }
                }
            }

            return result;
        }

        public async Task<int> Add(DesignBriefSourceReferenceModel model)
        {
            int result = 0;

            using (var context = NewContext())
            {
                var entity = MapToEntity(model);

                context.DesignBriefPolicyRequirementSources.AddObject(entity);
                await context.SaveChangesAsync();

                model.Id = entity.ID;

                result = 1;
            }

            return result;
        }

        public async Task<DesignBriefSourceReferenceModel> GetOneModelAsync(int id)
        {
            var entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                var model = MapToModel(entity);
                return model;
            }

            return default(DesignBriefSourceReferenceModel);
        }

        public async Task<int> Update(DesignBriefSourceReferenceModel model)
        {
            int result = 0;

            var entity = await GetOneEntityAsync(model.Id);

            if (entity != null)
            {
                entity.Description = model.Description;

                result = 1;
            }

            return result;
        }

        public async Task<int> Delete(int id)
        {
            int result = 0;

            var entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                _Context.DesignBriefPolicyRequirementSources.DeleteObject(entity);

                result = 1;
            }

            return result;
        }
    }
}
