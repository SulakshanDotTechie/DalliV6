using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DALI.SearchEngine.Models
{
    public class LiorParametersModel : ParametersModel
    {
        public Guid[] LocalAuthorityId { get; set; }
        public int[] Chapters { get; set; }
        public int[] Levels { get; set; }
        public int[] Subjects { get; set; }
        public int[] Areas { get; set; }
        public Guid[] Locations { get; set; }
        public int[] ChildSubjects { get; set; }
        public int[] Strictnesses { get; set; }
        public int[] Attachments { get; set; }
        public int[] SourceDocuments { get; set; }
        public int[] Themes { get; set; }
        public string Keyword { get; set; }
        public string Description { get; set; }
        public int PolicyRequirementId { get; set; }
        public string Remark { get; set; }
        public string Comment { get; set; }
        public string Extension { get; set; }
        public bool TownSpecificAreas { get; set; }
        public int Version { get; set; }
    }

    public class DesignBriefParametersModel : LiorParametersModel
    {
        public int DesignBriefId { get; set; }
    }
}