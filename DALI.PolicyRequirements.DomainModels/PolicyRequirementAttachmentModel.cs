using DALI.PolicyRequirements.DomainModelInterfaces;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.Serialization;

namespace DALI.PolicyRequirements.DomainModels
{
    [KnownType(typeof(PolicyRequirementAttachmentModel))]
    public class PolicyRequirementAttachmentModel : MediaModel, IPolicyRequirementAttachmentModel, IPolicyRequirementAttachmentExportModel
    {
        public int PolicyRequirementId { get; set; }
        public bool IsAssigned { get; set; }

        public override string Display()
        {
            return base.Display() + "_LIOR";
        }
    }
}
