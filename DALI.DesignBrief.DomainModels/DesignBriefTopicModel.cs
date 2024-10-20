using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DALI.DesignBrief.DomainModels
{
    [KnownType(typeof(DesignBriefTopicModel))]
    public class DesignBriefTopicModel
    {
        public int DesignBriefId { get; set; }
        public int Id { get; set; }
        public int LiorId { get; set; }
        public int VersionId { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public string OwnerFullName { get; set; }
        public string OwnerEmailAddress { get; set; }
        public bool Imported { get; set; }
        public bool InUse { get; set; }
        public string ChapterNumber
        {
            get; set;
        }

        public string FullDescription
        {
            get
            {
                string chapterNr = ChapterNumber;

                if (string.IsNullOrEmpty(chapterNr))
                {
                    chapterNr = LiorId < 10 ? "0" + LiorId.ToString() : LiorId.ToString();
                }

                return string.Format("{0}. {1}", chapterNr, Description);
            }
        }
    }
}
