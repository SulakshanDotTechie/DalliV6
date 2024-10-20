using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DALI.UserAccount.Models
{
    public class UserRole
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
    }

    public class UserInfo
    {
        public bool IsReader { get; set; }
        public bool IsOnlyReader { get; set; }
        public bool IsOwner { get; set; }
        public bool IsPublisher { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsWDC { get; set; }
        public bool IsPAC { get; set; }
        public bool IsTimesheetManager { get; set; }
        public bool IsTimesheetProjectManager { get; set; }
        public bool IsDesignBriefEditor { get; set; }
        public bool IsDesignBriefReader { get; set; }

        public bool HasAvatar { get; set; }

        public bool AccessToLior { get { return (IsReader || IsOwner || IsPublisher); } }
        public bool AccessToIncidents { get { return (IsWDC || IsPAC); } }
        public bool AccessToTimesheet { get; set; }
        public bool AccessToDesignBrief { get { return AccessToLior || IsDesignBriefEditor || IsDesignBriefReader; } }

        public string UserName { get; set; }
        public string Name { get; set; }

        public string AccountNameInfo
        {
            get
            {
                return string.IsNullOrEmpty(Name) ? UserName : string.Format("{0} ({1})", Name, UserName);
            }
        }
    }

    public class UserModel
    {
        private UserRole _Role;

        public Guid Id { get; set; }
        public string Description { get; set; }

        public string UserName { get; set; }
        public string EmailAddress { get; set; }


        public UserRole Role
        {
            get
            {
                if (_Role == null)
                    _Role = new UserRole();
                return _Role;
            }
            set
            {
                _Role = value;
            }
        }
    }
}