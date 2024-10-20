using DALI.PolicyRequirements.DomainModelInterfaces;
using DALI.Topics.SharedInfrastructure;
using System.Runtime.Serialization;

namespace DALI.PolicyRequirements.DomainModels
{
    [KnownType(typeof(PolicyRequirementChildSubjectModel))]
    public class PolicyRequirementChildSubjectModel : PolicyRequirementSubjectModel, IPolicyRequirementChildSubjectModel, IPolicyRequirementChildSubjectExportModel, IChildSubjectTopicModel
    {
        public string ChildSubject => Description;
    }
}
