using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.Topics.SharedInfrastructure
{
    public interface IChapterTopicModel
    {
        int Id { get; }
        string ChapterNumber { get; set; }
        string FullChapterDescription { get; }
        string Description { get; set; }
        string Owner { get; set; }
        string OwnerEmailAddress { get; set; }
    }
}
