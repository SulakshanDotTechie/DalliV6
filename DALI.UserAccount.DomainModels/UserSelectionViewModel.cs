using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DALI.UserAccount.Models
{
    public class UserSelectionViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [IgnoreDataMember]
        public IList<string> Roles { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string Organization { get; set; }
        public string Id { get; set; }

        public UserSelectionViewModel(string firstName, string lastName, IList<string> roles) : this(firstName, lastName)
        {
            Roles = roles ?? new List<string>();
        }

        public UserSelectionViewModel(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
            Roles = new List<string>();
        }

        public UserSelectionViewModel()
        {
            Roles = new List<string>();
        }

        private string AccountName
        {
            get
            {
                return !string.IsNullOrEmpty(FirstName) ? string.Format("{0} {1} ({2})", FirstName, LastName, UserName) : UserName;
            }
        }

        public string FullAccountName
        {
            get
            {
                return string.Format("{0} - {1} - {2}", FullName, AssignedRoles, Email.ToLower());
            }
        }


        public string AssignedRoles
        {
            get
            {
                var roles = (Roles != null && Roles.Count > 0) ? string.Join(", ", Roles) : "";

                return roles;
            }
        }

        public string FullName
        {
            get
            {
                return !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) ? string.Format("{0} {1}", LastName, FirstName) : UserName;
            }
        }

        public string Value
        {
            get
            {
                return UserName;
            }
        }

        public string Label
        {
            get
            {
                return FullAccountName;
            }
        }

        public string Description
        {
            get
            {
                return Label;
            }
        }

        public bool HasSubscription
        {
            get;
            set;
        }

        public bool AccountIsBlocked
        {
            get;
            set;
        }
    }
}
