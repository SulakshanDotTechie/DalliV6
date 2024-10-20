using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.ExportEngine.Models
{
    public interface IPolicyRequirementLocationExportModel
    {
        Guid Id { get; set; }
        string Description { get; set; }
        int OrderIndex { get; set; }
    }
}
