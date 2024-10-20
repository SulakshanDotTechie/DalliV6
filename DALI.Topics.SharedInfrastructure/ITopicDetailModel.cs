using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.Topics.SharedInfrastructure
{
    public interface ITopicDetailModel
    {
        int Id { get; set; }
        int VersionId { get; set; }
        int? OrderIndex { get; set; }
        string Description { get; set; }
        string Owner { get; set; }

        Guid LocationId { get; }
        int AreaId { get; }

        ILocalAuthorityTopicModel LocalAuthority { get; }
        IChapterTopicModel Chapter { get; }
        ILevelTopicModel Level { get; }
        ILocationTopicModel Location { get; }
        IAreaTopicModel Area { get; }
        ISubjectTopicModel Subject { get; }
        IChildSubjectTopicModel ChildSubject { get; }

        bool IsVirtualModel { get; }
    }
}
