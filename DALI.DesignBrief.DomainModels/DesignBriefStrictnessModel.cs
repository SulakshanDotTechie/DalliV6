using DALI.ExportEngine.Models;
using DALI.Topics.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DALI.DesignBrief.DomainModels
{
    [KnownType(typeof(DesignBriefSeverityModel))]
    public class DesignBriefSeverityModel : IPolicyRequirementSeverityExportModel, ISeverityTopicModel
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string ShortName { get; set; }

        public string Clarification { get; set; }
    }
}
