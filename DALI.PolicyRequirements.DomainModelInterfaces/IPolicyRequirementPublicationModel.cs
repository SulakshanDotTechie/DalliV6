using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.DomainModels
{
    public interface IPolicyRequirementPublicationModel : IVersionedModel<int>
    {
        string PublishedBy { get; set; }

        bool? IsPublished { get; set; }

        DateTime StartTime { get; set; }
        DateTime? EndTime { get; set; }
    }
}