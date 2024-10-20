using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DALI.PolicyRequirements.DomainModels
{
    public class FileExportModel
    {
        public string ExportTimeStamp { get; set; }
        public string FileName { get; set; }
        public bool IncludeAttachments { get; set; }
        public string ExportFormat { get; set; }
        public int Version { get; set; }
        public string ExportModule { get; set; }
    }
}