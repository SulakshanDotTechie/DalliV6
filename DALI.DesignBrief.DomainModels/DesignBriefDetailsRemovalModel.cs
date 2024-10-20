using DALI.ExportEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DALI.DesignBrief.DomainModels
{
    [KnownType(typeof(DesignBriefDetailsRemovalModel))]
    public class DesignBriefDetailsRemovalModel
    {
        public int DesignBriefId { get; set; }
        public string UserName { get; set; }

        public List<int> Items { get; set; }
    }
}
