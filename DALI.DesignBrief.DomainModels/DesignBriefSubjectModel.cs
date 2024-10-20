using DALI.ExportEngine.Models;
using DALI.Topics.Infrastructure.Models;
using System;
using System.Runtime.Serialization;
namespace DALI.DesignBrief.DomainModels
{
    [KnownType(typeof(DesignBriefSubjectModel))]
    public class DesignBriefSubjectModel : IPolicyRequirementSubjectExportModel, ISubjectTopicModel
    {
        public int Id { get; set; }

        public int DesignBriefId { get; set; }

        public int VersionId { get; set; }

        public string Description { get; set; }
    }
}
