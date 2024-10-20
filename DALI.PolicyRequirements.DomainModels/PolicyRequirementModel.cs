using System;
using System.Net;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using DALI.PolicyRequirements.DomainModelInterfaces;
using DALI.Topics.SharedInfrastructure;
using DALI.SearchEngine.Models;
using System.Web.Mvc;
using DALI.Common.Helpers;

namespace DALI.PolicyRequirements.DomainModels
{
    [KnownType(typeof(PolicyRequirementModel))]
    public class PolicyRequirementModel : PolicyRequirementMainPathModel, IPolicyRequirementModel, ISearchModel, IPolicyRequirementExportModel, ITopicDetailModel
    {
        private List<PolicyRequirementSeverityModel> _Severities;
        private List<PolicyRequirementAttachmentModel> _Attachments;
        private List<PolicyRequirementSourceDocumentModel> _SourceDocuments;

        public int Id { get; set; }

        public int VersionId { get; set; }

        public int? GroupIndex { get; set; }

        public int? OrderIndex { get; set; }

        [AllowHtml]
        public string Description { get; set; }

        public bool Active { get; set; }

        public Guid? UniqueID { get; set; }

        public int HasAttachments { get; set; }

        public int HasSourceReferences { get; set; }

        public string Owner { get; set; }

        [IgnoreDataMember]
        public DateTime? CreatedDate { get; set; }

        [IgnoreDataMember]
        public DateTime? ModifiedDate { get; set; }

        [IgnoreDataMember]
        public string CreatedBy { get; set; }

        [IgnoreDataMember]
        public string ModifiedBy { get; set; }

        public DateTime? ExpirationDate
        {
            get;
            set;
        }

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public override PolicyRequirementLocalAuthorityModel LocalAuthority
        {
            get
            {
                if (_LocalAuthority == null)
                    _LocalAuthority = new PolicyRequirementLocalAuthorityModel();
                return _LocalAuthority;
            }
            set
            {
                _LocalAuthority = value;
            }
        }

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public override PolicyRequirementChapterModel Chapter
        {
            get
            {
                if (_Chapter == null)
                    _Chapter = new PolicyRequirementChapterModel();
                return _Chapter;
            }
            set
            {
                _Chapter = value;
            }
        }

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public override PolicyRequirementLevelModel Level
        {
            get
            {
                if (_Level == null)
                    _Level = new PolicyRequirementLevelModel();
                return _Level;
            }
            set
            {
                _Level = value;
            }
        }

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public override PolicyRequirementLocationModel Location
        {
            get
            {
                if (_Location == null)
                    _Location = new PolicyRequirementLocationModel();
                return _Location;
            }
            set
            {
                _Location = value;
            }
        }

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public override PolicyRequirementAreaModel Area
        {
            get
            {
                if (_Area == null)
                    _Area = new PolicyRequirementAreaModel();
                return _Area;
            }
            set
            {
                _Area = value;
            }
        }

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public override PolicyRequirementSubjectModel Subject
        {
            get
            {
                if (_Subject == null)
                    _Subject = new PolicyRequirementSubjectModel();
                return _Subject;
            }
            set
            {
                _Subject = value;
            }
        }

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public override PolicyRequirementChildSubjectModel ChildSubject
        {
            get
            {
                if (_ChildSubject == null)
                    _ChildSubject = new PolicyRequirementChildSubjectModel();
                return _ChildSubject;
            }
            set
            {
                _ChildSubject = value;
            }
        }

        [AllowHtml]
        public string DescriptionAsHtml
        {
            get
            {
                var html = WebUtility.HtmlDecode(Description);
                html = HtmlStringHelper.ReplacePTag(html, RemovalAction.BeginEndOnly);
                html = HtmlStringHelper.ReplaceDivTag(html, RemovalAction.BeginEndOnly);

                return html;
            }
            set
            {
                var des = WebUtility.HtmlDecode(value);
                des = HtmlStringHelper.ReplacePTag(des, RemovalAction.BeginEndOnly);
                des = HtmlStringHelper.ReplaceDivTag(des, RemovalAction.BeginEndOnly);

                Description = WebUtility.HtmlEncode(des);
            }
        }

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public List<PolicyRequirementSeverityModel> Strictnesses
        {
            get
            {
                if (_Severities == null)
                    _Severities = new List<PolicyRequirementSeverityModel>();
                return _Severities;
            }
        }

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public List<PolicyRequirementAttachmentModel> Attachments
        {
            get
            {
                if (_Attachments == null)
                    _Attachments = new List<PolicyRequirementAttachmentModel>();
                return _Attachments;
            }
        }

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public List<PolicyRequirementSourceDocumentModel> SourceDocuments
        {
            get
            {
                if (_SourceDocuments == null)
                    _SourceDocuments = new List<PolicyRequirementSourceDocumentModel>();
                return _SourceDocuments;
            }
        }

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public IPolicyRequirementLocalAuthorityExportModel LocalAuthorityExportModel => _LocalAuthority;

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public IPolicyRequirementChapterExportModel ChapterExportModel => _Chapter;

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public IPolicyRequirementLevelExportModel LevelExportModel => _Level;

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public IPolicyRequirementLocationExportModel LocationExportModel => _Location;

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public IPolicyRequirementAreaExportModel AreaExportModel => _Area;

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public IPolicyRequirementSubjectExportModel SubjectExportModel => _Subject;

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public IPolicyRequirementChildSubjectExportModel ChildSubjectExportModel => _ChildSubject;

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        ILocalAuthorityTopicModel ITopicDetailModel.LocalAuthority => _LocalAuthority;

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        IChapterTopicModel ITopicDetailModel.Chapter => _Chapter;

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        ILevelTopicModel ITopicDetailModel.Level => _Level;

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        ILocationTopicModel ITopicDetailModel.Location => _Location;

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        IAreaTopicModel ITopicDetailModel.Area => _Area;

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        ISubjectTopicModel ITopicDetailModel.Subject => _Subject;

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        IChildSubjectTopicModel ITopicDetailModel.ChildSubject => _ChildSubject;

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public bool TownSpecificAreas => _Area.IsTownSpecific;

        public bool IsVirtualModel
        {
            get
            {
                return Id >= 10000;
            }
        }
    }

    [KnownType(typeof(PolicyRequirementFullModel))]
    public class PolicyRequirementFullModel : PolicyRequirementMainPathModel, IPolicyRequirementModel
    {
        private List<PolicyRequirementSeverityModel> _Severities;
        private List<PolicyRequirementAttachmentModel> _Attachments;
        private List<PolicyRequirementSourceDocumentModel> _SourceDocuments;

        public int Id { get; set; }

        public int VersionId { get; set; }

        public int? GroupIndex { get; set; }

        public int? OrderIndex { get; set; }

        [AllowHtml]
        public string Description { get; set; }

        public bool Active { get; set; }

        public string Owner { get; set; }

        [AllowHtml]
        public string DescriptionAsHtml
        {
            get
            {
                var html = WebUtility.HtmlDecode(Description);
                html = HtmlStringHelper.ReplacePTag(html, RemovalAction.BeginEndOnly);
                html = HtmlStringHelper.ReplaceDivTag(html, RemovalAction.BeginEndOnly);

                return html;
            }
            set
            {
                var des = WebUtility.HtmlDecode(value);
                des = HtmlStringHelper.ReplacePTag(des, RemovalAction.BeginEndOnly);
                des = HtmlStringHelper.ReplaceDivTag(des, RemovalAction.BeginEndOnly);

                Description = WebUtility.HtmlEncode(des);
            }
        }

        public Guid? UniqueID { get; set; }

        public int HasAttachments { get; set; }

        public int HasSourceReferences { get; set; }

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public List<PolicyRequirementSeverityModel> Strictnesses
        {
            get
            {
                if (_Severities == null)
                    _Severities = new List<PolicyRequirementSeverityModel>();
                return _Severities;
            }
            set
            {
                _Severities = value;
            }
        }

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public List<PolicyRequirementAttachmentModel> Attachments
        {
            get
            {
                if (_Attachments == null)
                    _Attachments = new List<PolicyRequirementAttachmentModel>();
                return _Attachments;
            }
            set
            {
                _Attachments = value;
            }
        }

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public List<PolicyRequirementSourceDocumentModel> SourceDocuments
        {
            get
            {
                if (_SourceDocuments == null)
                    _SourceDocuments = new List<PolicyRequirementSourceDocumentModel>();
                return _SourceDocuments;
            }
            set
            {
                _SourceDocuments = value;
            }
        }

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public DateTime? CreatedDate { get; set; }

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public DateTime? ModifiedDate { get; set; }

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public string CreatedBy { get; set; }

        // Ignored by both Json.NET and DataContractSerializer
        [IgnoreDataMember]
        public string ModifiedBy { get; set; }

        public DateTime? ExpirationDate
        {
            get;
            set;
        }
    }
}
