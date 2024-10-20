using DALI.ExportEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DALI.DesignBrief.DomainModels
{
    public class DesignBriefDetailsModel
    {
        List<DesignBriefPolicyRequirementModel> _Items;

        public int DesignBriefId { get; set; }

        public string UserName { get; set; }

        public List<DesignBriefPolicyRequirementModel> Items
        {
            get
            {
                if (_Items == null)
                    _Items = new List<DesignBriefPolicyRequirementModel>();

                return _Items;
            }
            set
            {
                _Items = value;
            }
        }
    }
}
