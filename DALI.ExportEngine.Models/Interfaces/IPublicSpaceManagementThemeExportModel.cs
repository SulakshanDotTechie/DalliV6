using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.ExportEngine.Models
{
    public interface IPublicSpaceManagementThemeExportModel
    {
        int Id { get; set; }
        string Description { get; set; }
        string Abbreviation { get; set; }
    }
}
