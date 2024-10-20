using DALI.ExportEngine.Models;
using System.Runtime.Serialization;

namespace DALI.DesignBrief.DomainModels
{
    public class DesignBriefSourceReferenceModel : IDesignBriefSourceReferenceExportModel
    {
        public int Id { get; set; }
        public int? LiorId { get; set; }
        public string StorageLocation { get; set; }
        public string Description { get; set; }
        public int PolicyRequirementId { get; set; }
        public int DesignBriefPolicyRequirementId { get; set; }
    }
}
