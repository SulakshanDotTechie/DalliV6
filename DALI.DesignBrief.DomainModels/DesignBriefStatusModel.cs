using DALI.DesignBrief.BusinessTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DALI.DesignBrief.DomainModels
{
    [KnownType(typeof(DesignBriefStatusModel))]
    public class DesignBriefStatusModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string ResourceName
        {
            get
            {
                return Id > 0 ? DesignBriefStatusEnumExt.AsDisplayString(Id) : string.Empty;
            }
        }
    }
}
