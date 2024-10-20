using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DALI.PolicyRequirements.DomainModels
{
    [KnownType(typeof(UserModel))]
    public class UserModel
    {
        public string UserName { get; set; }

        public int Id { get; set; }

        public int VersionId { get; set; }

        public string Description { get; set; }

        public bool InActive { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }
    }
}