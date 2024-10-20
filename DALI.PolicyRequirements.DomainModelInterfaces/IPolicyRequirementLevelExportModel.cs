using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.DomainModelInterfaces
{
    public interface IPolicyRequirementLevelExportModel
    {
        int Id { get; set; }
        string Position { get; set; }
        string Description { get; set; }
        string Name { get; set; }
    }
}
