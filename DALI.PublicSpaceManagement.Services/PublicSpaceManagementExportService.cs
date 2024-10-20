using DALI.ExportEngine.Models;
using DALI.PolicyRequirements.DomainModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DALI.PublicSpaceManagement.Services
{
    public class PublicSpaceManagementExportService
    {
        protected PublicSpaceManagementServices _Services;
        protected PolicyRequirementPublicationModel _PublicationInfo;
        public delegate string PublishedByFullNameEventHandler(string userName);
        public delegate string ThemeEventHandler(IPolicyRequirementExportModel detail);
        public event PublishedByFullNameEventHandler PublishedByFullName;

        public PublicSpaceManagementExportService(PublicSpaceManagementServices services)
        {
            _Services = services;

            var task = Task.Run(async () => await _Services.GetPublicationInfo());

            _PublicationInfo = task.Result;
        }

        protected virtual string OnPublishedByFullName(string userName)
        {
            return PublishedByFullName?.Invoke(userName);
        }

        public int[] GetChapterIds(int currentVersion)
        {
            var task = Task.Run(async () => await _Services.GetChapters(currentVersion));

            var models = task.Result;

            return models.Select(e => e.Id).Distinct().ToArray();
        }

        public List<IPolicyRequirementLevelPropertyExportModel> OnSetLevelProperties(IPolicyRequirementLevelExportModel level)
        {
            var task = Task.Run(async () => await _Services.GetLevelProperties(level.Id));

            return task.Result.ToList<IPolicyRequirementLevelPropertyExportModel>();
        }

        public List<IPolicyRequirementSeverityExportModel> OnSetSeverities(IPolicyRequirementExportModel detail)
        {
            var task = Task.Run(async () => await _Services.GetSeverities(detail.Id, detail.VersionId));

            return task.Result.ToList<IPolicyRequirementSeverityExportModel>();
        }

        public List<IPolicyRequirementSourceReferenceExportModel> OnSetSourceReferences(IPolicyRequirementExportModel detail)
        {
            var task = Task.Run(async () => await _Services.GetSourceDocuments(detail.Id, detail.VersionId));

            return task.Result.ToList<IPolicyRequirementSourceReferenceExportModel>();
        }

        public List<IPolicyRequirementAttachmentExportModel> OnSetAttachments(IPolicyRequirementExportModel detail)
        {
            var task = Task.Run(async () => await _Services.GetAttachments(detail.Id, detail.VersionId));

            return task.Result.ToList<IPolicyRequirementAttachmentExportModel>();
        }

        public IPublicSpaceManagementThemeExportModel[] OnGetAllThemes()
        {
            var task = Task.Run(async () => await _Services.GetThemes());

            return task.Result;
        }

        public IPublicSpaceManagementThemeExportModel[] OnGetByTheme(IPolicyRequirementExportModel detail)
        {
            var task = Task.Run(async () => await _Services.GetByTheme(detail.Id, detail.VersionId));

            return task.Result;
        }

        public string OnSetPublishedBy(IPolicyRequirementExportModel detail)
        {
            return _PublicationInfo != null ? OnPublishedByFullName(_PublicationInfo.PublishedBy) : string.Empty;
        }

        public string OnSetPublishedEndTime(IPolicyRequirementExportModel detail)
        {
            return (_PublicationInfo != null && _PublicationInfo.EndTime.HasValue) ? _PublicationInfo.EndTime.Value.ToShortDateString() : string.Empty;
        }
    }
}
