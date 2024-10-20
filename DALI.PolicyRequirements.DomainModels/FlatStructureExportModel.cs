using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.DomainModels
{
    public class PolicyRequirementFlatStructureModel
    {
        public int Id { get; set; }
        public string Chapter { get; set; }
        public string Level { get; set; }
        public string Area { get; set; }
        public string Subject { get; set; }
        public string Location { get; set; }
        public string ChildSubject { get; set; }
        public string Description { get; set; }
        public string Attachments { get; set; }
        public string SourceDocuments { get; set; }
    }
}
