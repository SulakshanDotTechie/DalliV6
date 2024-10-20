using DALI.ExportEngine.Models;
using DALI.Topics.Infrastructure.Models;
using System;
using System.Runtime.Serialization;

namespace DALI.DesignBrief.DomainModels
{
    [KnownType(typeof(DesignBriefLevelModel))]
    public class DesignBriefLevelModel : IPolicyRequirementLevelExportModel, ILevelTopicModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }

        public int VersionId { get; set; }

        public string Description { get; set; }
    }
}
