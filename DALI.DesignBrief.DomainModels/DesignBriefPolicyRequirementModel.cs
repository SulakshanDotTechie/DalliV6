using DALI.Common.Helpers;
using DALI.ExportEngine.Models;
using DALI.PublicSpaceManagement.Shared;
using DALI.SearchEngine.Models;
using DALI.Topics.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DALI.DesignBrief.DomainModels
{
    [KnownType(typeof(DesignBriefPolicyRequirementBaseModel))]
    public class DesignBriefPolicyRequirementBaseModel
    {
        protected DesignBriefLocalAuthorityModel _LocalAuthority;
        protected DesignBriefChapterModel _Chapter;
        protected DesignBriefLevelModel _Level;
        protected DesignBriefLocationModel _Location;
        protected DesignBriefAreaModel _Area;
        protected DesignBriefSubjectModel _Subject;
        protected DesignBriefChildSubjectModel _ChildSubject;

        protected List<DesignBriefSeverityModel> _Severities;
        protected List<DesignBriefAttachmentBaseModel> _Attachments;
        protected List<DesignBriefSourceReferenceModel> _SourceDocuments;

        public int Id { get; set; }
        public Guid? UniqueID { get; set; }
        public int PolicyRequirementId { get; set; }
        public Guid PolicyRequirementUniqueId { get; set; }
        public Guid LocalAuthorityId
        {
            get;
            set;
        }
        public int ThemeId
        {
            get;
            set;
        }
        public int ChapterId
        {
            get;
            set;
        }
        public Guid LocationId
        {
            get;
            set;
        }
        public int LevelId
        {
            get;
            set;
        }
        public int AreaId
        {
            get;
            set;
        }
        public int SubjectId
        {
            get;
            set;
        }
        public int ChildSubjectId
        {
            get;
            set;
        }
        public int? OrderIndex { get; set; }
        public string Description { get; set; }
        public int VersionId { get; set; }
        public int? GroupIndex { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public int HasAttachments { get; set; }
        public int HasSourceReferences { get; set; }

        public DesignBriefLocalAuthorityModel LocalAuthority
        {
            get
            {
                if (_LocalAuthority == null)
                    _LocalAuthority = new DesignBriefLocalAuthorityModel();
                return _LocalAuthority;
            }
            set
            {
                _LocalAuthority = value;
            }
        }

        public DesignBriefChapterModel Chapter
        {
            get
            {
                if (_Chapter == null)
                    _Chapter = new DesignBriefChapterModel();
                return _Chapter;
            }
            set
            {
                _Chapter = value;
            }
        }
        public DesignBriefLevelModel Level
        {
            get
            {
                if (_Level == null)
                    _Level = new DesignBriefLevelModel();
                return _Level;
            }
            set
            {
                _Level = value;
            }
        }

        public DateTime? ExpirationDate
        {
            get;
            set;
        }

        public DesignBriefLocationModel Location
        {
            get
            {
                if (_Location == null)
                    _Location = new DesignBriefLocationModel();
                return _Location;
            }
            set
            {
                _Location = value;
            }
        }

        public DesignBriefAreaModel Area
        {
            get
            {
                if (_Area == null)
                    _Area = new DesignBriefAreaModel();
                return _Area;
            }
            set
            {
                _Area = value;
            }
        }
        public DesignBriefSubjectModel Subject
        {
            get
            {
                if (_Subject == null)
                    _Subject = new DesignBriefSubjectModel();
                return _Subject;
            }
            set
            {
                _Subject = value;
            }
        }

        public DesignBriefChildSubjectModel ChildSubject
        {
            get
            {
                if (_ChildSubject == null)
                    _ChildSubject = new DesignBriefChildSubjectModel();
                return _ChildSubject;
            }
            set
            {
                _ChildSubject = value;
            }
        }

        public string DescriptionAsHtml
        {
            get
            {
                return HtmlStringHelper.RemovePTag(WebUtility.HtmlDecode(Description), RemovalAction.BeginEndOnly);
            }
            set
            {
                var des = HtmlStringHelper.RemovePTag(WebUtility.HtmlDecode(value), RemovalAction.BeginEndOnly);
                Description = WebUtility.HtmlEncode(des);
            }
        }

        public string SemiPath
        {
            get
            {
                return string.Format("{0}|{1}|{2}|{3}", Chapter.FullChapterDescription, Level.Name, Location.Description, Area.Description);
            }
        }

        public string SemiPathKey
        {
            get
            {
                return string.Format("{0}_{1}_{2}_{3}_{4}", ChapterId, LevelId, LocationId, AreaId, SubjectId);
            }
        }

        public string Path
        {
            get
            {
                return string.Format("{0}|{1}|{2}|{3}|{4}|{5}", Chapter.FullChapterDescription, Level.Name, Location.Description, Area.Description, Subject.Description, ChildSubject.Description);
            }
        }

        public List<DesignBriefSeverityModel> Strictnesses
        {
            get
            {
                if (_Severities == null)
                    _Severities = new List<DesignBriefSeverityModel>();
                return _Severities;
            }
        }
        public List<DesignBriefAttachmentBaseModel> Attachments
        {
            get
            {
                if (_Attachments == null)
                    _Attachments = new List<DesignBriefAttachmentBaseModel>();
                return _Attachments;
            }
        }
        public List<DesignBriefSourceReferenceModel> SourceDocuments
        {
            get
            {
                if (_SourceDocuments == null)
                    _SourceDocuments = new List<DesignBriefSourceReferenceModel>();
                return _SourceDocuments;
            }
        }

        public string Owner
        {
            get;
            set;
        }
        public bool Active
        {
            get
            {
                return true;
            }

            set
            {
                return;
            }
        }
    }

    public class DesignBriefPolicyRequirementModel : DesignBriefPolicyRequirementBaseModel, ISearchModel, IPolicyRequirementExportModel, ITopicDetailModel
    {
        private ApprovalModel _ApprovalModel;

        [IgnoreDataMember]
        public IPolicyRequirementChapterExportModel ChapterExportModel => _Chapter;

        [IgnoreDataMember]
        public IPolicyRequirementLevelExportModel LevelExportModel => _Level;

        [IgnoreDataMember]
        public IPolicyRequirementAreaExportModel AreaExportModel => _Area;

        [IgnoreDataMember]
        public IPolicyRequirementSubjectExportModel SubjectExportModel => _Subject;

        [IgnoreDataMember]
        public IPolicyRequirementLocalAuthorityExportModel LocalAuthorityExportModel => _LocalAuthority;

        [IgnoreDataMember]
        public IPolicyRequirementLocationExportModel LocationExportModel => _Location;

        [IgnoreDataMember]
        public IPolicyRequirementChildSubjectExportModel ChildSubjectExportModel => _ChildSubject;

        [IgnoreDataMember]
        ILocalAuthorityTopicModel ITopicDetailModel.LocalAuthority => _LocalAuthority;

        [IgnoreDataMember]
        IChapterTopicModel ITopicDetailModel.Chapter => _Chapter;

        [IgnoreDataMember]
        ILevelTopicModel ITopicDetailModel.Level => _Level;

        [IgnoreDataMember]
        ILocationTopicModel ITopicDetailModel.Location => _Location;

        [IgnoreDataMember]
        IAreaTopicModel ITopicDetailModel.Area => _Area;

        [IgnoreDataMember]
        ISubjectTopicModel ITopicDetailModel.Subject => _Subject;

        [IgnoreDataMember]
        IChildSubjectTopicModel ITopicDetailModel.ChildSubject => _ChildSubject;

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public bool TownSpecificAreas
        {
            get
            {
                return _Area != null ? _Area.IsTownSpecific : false;
            }
        }

        public bool IsVirtualModel
        {
            get
            {
                return PolicyRequirementId >= 10000;
            }
        }

        public IApprovalModel Approval
        {
            get
            {
                if (_ApprovalModel == null)
                {
                    _ApprovalModel = new ApprovalModel();
                    _ApprovalModel.Clarification = "";
                }

                return _ApprovalModel;
            }
        }
    }
}
