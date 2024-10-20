using DALI.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DALI.PolicyRequirements.DomainModels
{
    [KnownType(typeof(PolicyRequirementModificationModel))]
    public class PolicyRequirementModificationModel : IPolicyRequirementModificationModel
    {
        private PolicyRequirementLocalAuthorityModel _LocalAuthority;
        private PolicyRequirementChapterModel _Chapter;
        private PolicyRequirementLevelModel _Level;
        private PolicyRequirementAreaModel _Area;
        private PolicyRequirementSubjectModel _Subject;
        private PolicyRequirementLocationModel _Location;
        private PolicyRequirementChildSubjectModel _ChildSubject;

        private List<PolicyRequirementSeverityModel> _Severities;
        private List<PolicyRequirementAttachmentModel> _Attachments;
        private List<PolicyRequirementSourceDocumentModel> _SourceDocuments;

        public int Id { get; set; }
        public int VersionId { get; set; }
        public int? GroupIndex { get; set; }
        public int? OrderIndex { get; set; }
        public bool QueuedForPublication { get; set; }

        public int PublicationStatus { get; set; }

        public PolicyRequirementLocalAuthorityModel LocalAuthority
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

        public PolicyRequirementChapterModel Chapter
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
        public PolicyRequirementLevelModel Level
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
        public PolicyRequirementAreaModel Area
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
        public PolicyRequirementSubjectModel Subject
        {
            get
            {
                if (_Subject == null)
                    _Subject = new PolicyRequirementSubjectModel();
                return _Subject;
            }
            set
            {
                _Subject = (PolicyRequirementSubjectModel)value;
            }
        }
        public PolicyRequirementLocationModel Location
        {
            get
            {
                if (_Location == null)
                    _Location = new PolicyRequirementLocationModel();
                return _Location;
            }
            set
            {
                _Location = (PolicyRequirementLocationModel)value;
            }
        }
        public PolicyRequirementChildSubjectModel ChildSubject
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

        public string Description { get; set; }
        public string CurrentDescription { get; set; }

        public bool Active { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

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

        [AllowHtml]
        public string CurrentDescriptionAsHtml
        {
            get
            {
                var html = WebUtility.HtmlDecode(CurrentDescription);
                html = HtmlStringHelper.ReplacePTag(html, RemovalAction.BeginEndOnly);
                html = HtmlStringHelper.ReplaceDivTag(html, RemovalAction.BeginEndOnly);

                return html;
            }
            set
            {
                var des = WebUtility.HtmlDecode(value);
                des = HtmlStringHelper.ReplacePTag(des, RemovalAction.BeginEndOnly);
                des = HtmlStringHelper.ReplaceDivTag(des, RemovalAction.BeginEndOnly);

                CurrentDescription = WebUtility.HtmlEncode(des);
            }
        }

        public string Owner { get; set; }

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
                return string.Format("{0}_{1}_{2}_{3}_{4}", Chapter.Id, Level.Id, Location.Id, Area.Id, Subject.Id);
            }
        }

        public string Path
        {
            get
            {
                return string.Format("{0}|{1}|{2}|{3}|{4}|{5}", Chapter.FullChapterDescription, Level.Name, Location.Description, Area.Description, Subject.Description, ChildSubject.Description);
            }
        }

        public List<PolicyRequirementSeverityModel> Strictnesses
        {
            get
            {
                if (_Severities == null)
                    _Severities = new List<PolicyRequirementSeverityModel>();
                return _Severities.ToList();
            }
            set { _Severities = value; }
        }

        public List<PolicyRequirementAttachmentModel> Attachments
        {
            get
            {
                if (_Attachments == null)
                    _Attachments = new List<PolicyRequirementAttachmentModel>();
                return _Attachments.ToList();
            }
            set { _Attachments = value; }
        }

        public List<PolicyRequirementSourceDocumentModel> SourceDocuments
        {
            get
            {
                if (_SourceDocuments == null)
                    _SourceDocuments = new List<PolicyRequirementSourceDocumentModel>();
                return _SourceDocuments.ToList();
            }
            set { _SourceDocuments = value; }
        }

        public string RejectMessage { get; set; }
        public string RejectedBy { get; set; }

        public Guid? UniqueID { get; set; }
        public int HasAttachments { get; set; }
        public int HasSourceReferences { get; set; }
    }
}
