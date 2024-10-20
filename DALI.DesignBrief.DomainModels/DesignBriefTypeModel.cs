using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DALI.DesignBrief.DomainModels
{
    [KnownType(typeof(DesignBriefTypeModel))]
    public class DesignBriefTypeModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
