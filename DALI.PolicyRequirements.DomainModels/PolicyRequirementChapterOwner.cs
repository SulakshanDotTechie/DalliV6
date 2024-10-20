using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.DomainModels
{
    public class PolicyRequirementChapterOwner
    {
        public string Owner
        {
            get;
            set;
        }

        public string FullName
        {
            get;
            set;
        }

        public int ChapterId { get; set; }
        public string FullChapterDescription { get; set; }
    }
}
