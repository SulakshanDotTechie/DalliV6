using DALI.ExportEngine.Models;
using DALI.Topics.Infrastructure.Models;
using System;
using System.Runtime.Serialization;

namespace DALI.DesignBrief.DomainModels
{
    [KnownType(typeof(DesignBriefChapterModel))]
    public class DesignBriefChapterModel : IPolicyRequirementChapterExportModel, IChapterTopicModel
    {
        public int Id { get; set; }

        public int DesignBriefId { get; set; }

        public string ChapterNumber { get; set; }

        public string FullChapterDescription
        {
            get
            {
                return string.Format("{0}. {1}", ChapterNumber, Description);
            }
        }

        public int VersionId { get; set; }

        public string Description { get; set; }

        public string Owner { get; set; }

        public string OwnerEmailAddress { get; set; }
    }
}
