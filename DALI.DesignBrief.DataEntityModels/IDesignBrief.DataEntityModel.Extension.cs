//-----------------------------------------------------------------------
// <copyright file="IPolicyRequirement.DataEntityModel.Extension.cs" company="Microsoft Corporation">
//     Copyright Microsoft Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;

namespace DALI.DesignBrief.DataEntityModels
{
    public static class IEntityExtension
    {
        public static int GetNextId(this IEntity entity, IDesignBriefDataEntityModelContext context)
        {
            TT_SYS_GENID idEntity = (from id in context.TT_SYS_GENID
                                     where id.TT_TABLENAME == entity.TableName
                                     select id).FirstOrDefault();
            if (idEntity == null)
                throw new ArgumentException(string.Format("Entity {0} not found in GenId table", entity.TableName));

            idEntity.TT_LASTGEN_ID++;

            return (int)idEntity.TT_LASTGEN_ID;
        }

        public static IEnumerable<TEntity> LoadedEntities<TEntity>(this DesignBriefDataEntityModels Context)
        {
            var entities = Context.ObjectStateManager
                .GetObjectStateEntries(EntityState.Added | EntityState.Modified | EntityState.Unchanged)
                .Where(e => !e.IsRelationship).Select(e => e.Entity).OfType<TEntity>();

            return entities;
        }
    }
}
