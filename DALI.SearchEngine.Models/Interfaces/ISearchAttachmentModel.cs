using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALI.SearchEngine.Models
{
    public interface ISearchAttachmentModel : ISearchMediaModel, ISearchVersionedModel<int>
    {
        int PolicyRequirementId { get; set; }
        bool IsAssigned { get; set; }
    }
}
