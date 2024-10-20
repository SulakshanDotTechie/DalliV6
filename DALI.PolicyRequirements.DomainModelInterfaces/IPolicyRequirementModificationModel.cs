using System;
using System.Collections.Generic;
namespace DALI.PolicyRequirements.DomainModels
{
    public interface IPolicyRequirementModificationModel : IPolicyRequirementModel
    {
        int PublicationStatus { get; set; }
        bool QueuedForPublication { get; set; }
        string RejectMessage { get; set; }
    }
}
