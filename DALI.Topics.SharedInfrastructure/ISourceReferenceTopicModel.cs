using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.Topics.SharedInfrastructure
{
    public interface ISourceReferenceTopicModel
    {
        int Id { get; set; }
        string Description { get; set; }
        string StorageLocation { get; set; }
    }

    //public interface IDesignBriefSourceReferenceExportModel : IPolicyRequirementSourceReferenceExportModel
    //{
    //    int? LiorId { get; set; }
    //}
}
