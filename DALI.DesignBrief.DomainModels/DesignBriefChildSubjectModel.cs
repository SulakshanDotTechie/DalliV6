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
    [KnownType(typeof(DesignBriefChildSubjectModel))]
    public class DesignBriefChildSubjectModel : IPolicyRequirementChildSubjectExportModel, IChildSubjectTopicModel
    {
        public int DesignBriefId { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public int VersionId { get; set; }

        public string ChildSubject => Description;
    }
}
