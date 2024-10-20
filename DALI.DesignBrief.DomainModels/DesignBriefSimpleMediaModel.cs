using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DALI.DesignBrief.DomainModels
{
    public class DesignBriefSimpleMediaModel
    {
        public int DesignBriefId { get; set; }
        public string ProjectCode { get; set; }
        public int PolicyRequirementId { get; set; }
        public string LabelName { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
    }
}