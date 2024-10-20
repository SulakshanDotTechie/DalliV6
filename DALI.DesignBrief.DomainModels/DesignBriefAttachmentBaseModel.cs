using DALI.ExportEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DALI.DesignBrief.DomainModels
{
    [KnownType(typeof(DesignBriefAttachmentBaseModel))]
    public class DesignBriefAttachmentBaseModel : IDesignBriefAttachmentExportModel
    {
        public int Id { get; set; }
        public int? LiorId { get; set; }
        public string FileName { get; set; }
        public string StorageLocation { get; set; }
        public string Description { get; set; }

        public string FileExtension
        {
            get
            {
                return System.IO.Path.GetExtension(FileName);
            }
        }

        public int PolicyRequirementId { get; set; }
        public bool IsAssigned { get; set; }
        public string BaseLocation { get; set; }
        public int DesignBriefPolicyRequirementId { get; set; }
    }
}
