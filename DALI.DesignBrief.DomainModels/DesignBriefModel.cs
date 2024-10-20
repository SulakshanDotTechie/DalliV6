using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DALI.DesignBrief.DomainModels
{
    [KnownType(typeof(DesignBriefModel))]
    public class DesignBriefModel
    {
        private List<DesignBriefTypeModel> _Types;
        private List<DesignBriefMemberModel> _Members;
        private List<DesignBriefMemberModel> _MembersBySubscription;
        private List<DesignBriefTopicModel> _Topics;
        private List<int> _Imports;
        private DesignBriefAgreementModel _Agreement;
        private DesignBriefStatusModel _Status;
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public List<DesignBriefTypeModel> Types
        {
            get
            {
                if (_Types == null)
                    _Types = new List<DesignBriefTypeModel>();

                return _Types;
            }
            set
            {
                _Types = value;
            }
        }
        public DesignBriefAgreementModel Agreement
        {
            get
            {
                if (_Agreement == null)
                    _Agreement = new DesignBriefAgreementModel();

                return _Agreement;
            }
            set
            {
                _Agreement = value;
            }
        }
        public DesignBriefStatusModel Status
        {
            get
            {
                if (_Status == null)
                    _Status = new DesignBriefStatusModel();

                return _Status;
            }
            set
            {
                _Status = value;
            }
        }
        public List<DesignBriefMemberModel> Members
        {
            get
            {
                if (_Members == null)
                    _Members = new List<DesignBriefMemberModel>();

                return _Members;
            }
            set
            {
                _Members = value;
            }
        }
        public List<DesignBriefMemberModel> MembersBySubscription
        {
            get
            {
                if (_MembersBySubscription == null)
                    _MembersBySubscription = new List<DesignBriefMemberModel>();

                return _MembersBySubscription;
            }
            set
            {
                _MembersBySubscription = value;
            }
        }
        public List<DesignBriefTopicModel> Topics
        {
            get
            {
                if (_Topics == null)
                    _Topics = new List<DesignBriefTopicModel>();

                return _Topics;
            }
            set
            {
                _Topics = value;
            }
        }
        public List<int> Imports
        {
            get
            {
                if (_Imports == null)
                    _Imports = new List<int>();

                return _Imports;
            }
            set
            {
                _Imports = value;
            }
        }
        public string Manager { get; set; }
        public string ManagerFullName { get; set; }
        public string CreatedByFullName { get; set; }
        public int? BaseId { get; set; }
        public int Version { get; set; }
        public string ProjectNumber
        {
            get
            {
                return BaseId > 0 ? BaseId.Value.ToString("D6") + "-" + Version : Id.ToString("D6") + "-" + Version;
            }
        }
        public int PublicationVersion { get; set; }
        public string Comment { get; set; }
    }
}
