using DALI.PolicyRequirements.DomainModelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALI.PolicyRequirements.DomainModels
{
    public interface IPolicyRequirementAttachmentModel : IMediaModel, IVersionedModel<int>
    {
        int PolicyRequirementId { get; set; }
        bool IsAssigned { get; set; }
    }
}
