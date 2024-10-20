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

namespace DALI.PolicyRequirements.DataEntityModels
{
    public static class IEntityExtension
    {
        public static async Task<int> GetNextIdAsync(this IEntity entity, IPolicyRequirementDataEntityModelContext context)
        {
            TT_SYS_GENID idEntity = await (from id in context.TT_SYS_GENID
                                           where id.TT_TABLENAME == entity.TableName
                                           select id).FirstOrDefaultAsync();
            if (idEntity == null)
                throw new ArgumentException(string.Format("Entity {0} not found in GenId table", entity.TableName));

            idEntity.TT_LASTGEN_ID++;

            return (int)idEntity.TT_LASTGEN_ID;
        }
    }
}
