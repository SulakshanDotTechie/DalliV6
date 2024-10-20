using DALI.Topics.Infrastructure.Models;
using System.Collections.Generic;

namespace DALI.PublicSpaceManagement.DomainModels
{
    public class PublicSpaceManagementThemeReferenceModel
    {
        public PublicSpaceManagementThemeReferenceModel()
        {
            Theme = new PublicSpaceManagementThemeModel();
            PolicyRequirementIds = new List<int>();
        }

        public PublicSpaceManagementThemeModel Theme { get; }

        public List<int> PolicyRequirementIds { get; }

        public TopicResponseModel Results
        {
            get;
            set;
        }
    }
}
