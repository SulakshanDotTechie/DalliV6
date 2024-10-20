using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.ExportEngine.Models
{
    public interface IPolicyRequirementExportModel
    {
        int Id { get; set; }
        int VersionId { get; set; }
        int? OrderIndex { get; set; }
        string Description { get; set; }
        string Owner { get; set; }

        IPolicyRequirementLocalAuthorityExportModel LocalAuthorityExportModel { get; }
        IPolicyRequirementChapterExportModel ChapterExportModel { get; }
        IPolicyRequirementLevelExportModel LevelExportModel { get; }
        IPolicyRequirementLocationExportModel LocationExportModel { get; }
        IPolicyRequirementAreaExportModel AreaExportModel { get; }
        IPolicyRequirementSubjectExportModel SubjectExportModel { get; }
        IPolicyRequirementChildSubjectExportModel ChildSubjectExportModel { get; }
    }
}
