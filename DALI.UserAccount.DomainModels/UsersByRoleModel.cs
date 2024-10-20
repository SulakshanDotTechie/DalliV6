using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.UserAccount.Models
{
    public class UsersByRoleModel
    {
        public UsersByRoleModel(string role, UserSelectionViewModel[] users)
        {
            Role = role;
            Users = users;
        }

        public string Role { get; private set; }

        public UserSelectionViewModel[] Users
        {
            get;
            private set;
        }
    }
}
