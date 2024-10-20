using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALI.PolicyRequirements.DomainModels
{
    public interface IVersionedModel<TIdType>
    {
        TIdType Id { get; set; }

        int VersionId { get; set; }

        bool Active { get; set; }

        DateTime? CreatedDate { get; set; }

        DateTime? ModifiedDate { get; set; }

        string CreatedBy { get; set; }

        string ModifiedBy { get; set; }

        string Description { get; set; }
    }
}
