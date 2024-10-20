using DALI.ExportEngine.Models;
using DALI.Topics.SharedInfrastructure;
using System;
using System.Runtime.Serialization;

namespace DALI.DesignBrief.DomainModels
{
    [KnownType(typeof(DesignBriefLocationModel))]
    public class DesignBriefLocationModel : IPolicyRequirementLocationExportModel, ILocationTopicModel
    {
        public Guid Id { get; set; }
        public string Description { get; set; }

        public bool FetchByDefault { get; set; }

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public string Location => Description;

        public int OrderIndex { get; set; }
    }
}
