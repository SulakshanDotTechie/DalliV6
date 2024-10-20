using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.DomainModelInterfaces
{
    public interface IPolicyRequirementChapterExportModel
    {
        int Id { get; }
        string ChapterNumber { get; set; }
        string FullChapterDescription { get; }
        string Description { get; set; }
        string Owner { get; set; }
        string OwnerEmailAddress { get; set; }
    }
}
