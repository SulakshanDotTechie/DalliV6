using DALI.ExportEngine.Models;
using DALI.Topics.SharedInfrastructure;

namespace DALI.PublicSpaceManagement.DomainModels
{
    public class PublicSpaceManagementThemeModel : IThemeTopicModel, IPublicSpaceManagementThemeExportModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Abbreviation { get; set; }
    }
}
