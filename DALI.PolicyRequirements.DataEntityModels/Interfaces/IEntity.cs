using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DALI.PolicyRequirements.DataEntityModels
{
    public interface IEntity
    {
        string TableName { get; }
    }
}
