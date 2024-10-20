using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DALI.DesignBrief.DataEntityModels
{
    public interface IEntity
    {
        string TableName { get; }
    }
}
