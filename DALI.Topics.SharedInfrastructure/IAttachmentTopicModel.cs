using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.Topics.SharedInfrastructure
{
    public interface IAttachmentTopicModel
    {
        int Id { get; set; }
        string Description { get; set; }
        int PolicyRequirementId { get; set; }
        bool IsAssigned { get; set; }

        string BaseLocation { get; set; }
        string FileName { get; set; }
    }

    //public interface IDesignBriefAttachmentExportModel : IPolicyRequirementAttachmentExportModel
    //{
    //    int? LiorId { get; set; }
    //}
}
