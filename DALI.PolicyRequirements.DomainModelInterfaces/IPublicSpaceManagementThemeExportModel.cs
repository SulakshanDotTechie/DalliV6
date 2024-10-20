using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.DomainModelInterfaces
{
    public interface IPublicSpaceManagementThemeExportModel
    {
        int Id { get; set; }
        string Description { get; set; }
        string Abbreviation { get; set; }
    }
}
