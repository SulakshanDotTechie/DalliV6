using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DALI.DesignBrief.DomainModels;
using DALI.DesignBrief.DataEntityModels;
using System.Transactions;
using System.IO;

namespace DALI.DesignBrief.Repositories
{
    public class DesignBriefAttachmentRepository : Repository
    {
        public DesignBriefAttachmentRepository(IDesignBriefDataEntityModelContext context) : base(context)
        {

        }

        private DesignBriefAttachmentModel MapToModel(DesignBriefAttachment entity)
        {
            DesignBriefAttachmentModel model = new DesignBriefAttachmentModel();

            model.Id = entity.Id;
            model.DesignBriefId = entity.DesignBriefId;
            model.Description = entity.Description;
            model.FileName = entity.FileName;
            model.DesignBriefCode = entity.DesignBriefs.Code;

            return model;
        }

        private DesignBriefAttachment MapToEntity(DesignBriefAttachmentModel model)
        {
            DesignBriefAttachment entity = new DesignBriefAttachment();

            entity.Id = model.Id;
            entity.DesignBriefId = model.DesignBriefId;
            entity.Description = model.LabelName;
            entity.FileName = Path.GetFileName(model.FileName);

            return entity;
        }

        private IQueryable<DesignBriefAttachment> GetAll()
        {
            return _Context.DesignBriefAttachments;
        }

        public async Task<DesignBriefAttachmentModel[]> GetAll(int id)
        {
            var entities = await GetAll().ToArrayAsync();

            var models = entities.Where(e => e.DesignBriefId == id).Select(e => MapToModel(e)).ToArray();

            return models;
        }

        private async Task<DesignBriefAttachment> GetOneEntityAsync(int id)
        {
            var entity = await GetAll().SingleOrDefaultAsync(e => e.DesignBriefId == id);

            return entity;
        }

        public async Task<int> Add(DesignBriefAttachmentModel model)
        {
            int result = 0;

            using (var context = NewContext())
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.Parse("60"), TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        var entity = MapToEntity(model);

                        context.DesignBriefAttachments.AddObject(entity);

                        await context.SaveChangesAsync().ConfigureAwait(false);

                        model.Id = entity.Id;

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

        public async Task<DesignBriefAttachmentModel> GetOneModelAsync(int id)
        {
            var entity = await GetOneEntityAsync(id);

            if (entity != null)
            {
                var model = MapToModel(entity);
                return model;
            }

            return default(DesignBriefAttachmentModel);
        }

        public async Task<DesignBriefAttachmentModel> GetOneModelAsync(string fileName)
        {
            var entity = await _Context.DesignBriefAttachments.SingleOrDefaultAsync(e => string.Compare(e.FileName, fileName, true) == 0);

            if (entity != null)
            {
                var model = MapToModel(entity);
                return model;
            }

            return default(DesignBriefAttachmentModel);
        }

        public async Task<int> Update(DesignBriefAttachmentModel model)
        {
            int result = 0;

            var entity = await GetOneEntityAsync(model.Id);

            if (entity != null)
            {
                entity.Description = model.LabelName;

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
                _Context.DesignBriefAttachments.DeleteObject(entity);

                result = 1;
            }

            return result;
        }
    }
}
