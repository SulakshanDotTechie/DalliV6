using DALI.ExportEngine.Models;
using DALI.Topics.Infrastructure.Models;

namespace DALI.DesignBrief.DomainModels.Models
{
    public class DesignBriefThemeModel : IThemeTopicModel, IPublicSpaceManagementThemeExportModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Abbreviation { get; set; }
    }
}
