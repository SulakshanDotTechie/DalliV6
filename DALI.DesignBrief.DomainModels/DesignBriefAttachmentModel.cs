using DALI.ExportEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DALI.DesignBrief.DomainModels
{
    [KnownType(typeof(DesignBriefAttachmentModel))]
    public class DesignBriefAttachmentModel : DesignBriefAttachmentBaseModel
    {
        public int DesignBriefId { get; set; }
        public string DesignBriefCode { get; set; }
        public string LabelName
        {
            get { return Description; }
            set { Description = value; }
        }
        public string Url { get; set; }
    }
}
