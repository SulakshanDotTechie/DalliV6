using DALI.ExportEngine.Models;
using System;
using System.Runtime.Serialization;
namespace DALI.DesignBrief.DomainModels
{
    public class DesignBriefLevelPropertyModel : IPolicyRequirementLevelPropertyExportModel
    {
        public int Sequence { get; set; }

        public int Id { get; set; }

        public int LevelId { get; set; }

        public int VersionId { get; set; }

        public string Description { get; set; }
    }
}
