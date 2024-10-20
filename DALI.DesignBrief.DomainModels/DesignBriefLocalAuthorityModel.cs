using DALI.ExportEngine.Models;
using DALI.Topics.Infrastructure.Models;
using System;

namespace DALI.DesignBrief.DomainModels
{
    public class DesignBriefLocalAuthorityModel : IPolicyRequirementLocalAuthorityExportModel, ILocalAuthorityTopicModel
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
    }
}
