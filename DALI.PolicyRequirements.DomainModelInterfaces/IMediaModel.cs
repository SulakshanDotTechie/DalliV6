using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.PolicyRequirements.DomainModelInterfaces
{
    public interface IMediaModel
    {
        string StorageLocation { get; set; }
        string BaseLocation { get; set; }
        string FileExtension { get; set; }
        string FileName { get; set; }
        string PDFLocation { get; set; }
        string ACADLocation { get; set; }

        bool IsUrl { get; set; }
        bool IsYoutubeUrl { get; set; }
        bool IsWindowsMediaVideo { get; set; }
        bool IsQuickTimeVideo { get; set; }
        bool IsAviVideo { get; set; }
        bool IsMsOfficeFile { get; set; }
        bool IsImage { get; set; }
        bool IsAcadFile { get; set; }
    }
}
