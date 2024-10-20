using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.DesignBrief.DomainModels
{
    public class DesignBriefReorderModel
    {
        public int DesignBriefId { get; set; }
        public List<int> OrderList { get; set; }
    }
}
