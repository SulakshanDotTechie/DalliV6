using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DALI.DesignBrief.DomainModels
{
    [KnownType(typeof(DesignBriefMemberModel))]
    public class DesignBriefMemberModel
    {
        public int DesignBriefId { get; set; }
        public string UserName { get; set; }
        public string OfficeInfo { get; set; }
        public bool Editor { get; set; }
        public string FullName { get; set; }
        public bool MemberBySubscription { get; set; }
    }
}
