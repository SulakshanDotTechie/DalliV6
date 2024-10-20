using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DALI.PolicyRequirements.DomainModelInterfaces;
using System.IO;
using System.Net;
using System.Runtime.Serialization;

namespace DALI.PolicyRequirements.DomainModels
{
    [KnownType(typeof(MediaModel))]
    public class MediaModel : IMediaModel, IVersionedModel<int>
    {
        public int Id { get; set; }
        public int VersionId { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string StorageLocation { get; set; }
        public string BaseLocation { get; set; }
        public string FileExtension { get; set; }
        public string PDFLocation { get; set; }
        public string ACADLocation { get; set; }
        public string FileName { get; set; }

        public bool Active { get; set; }
        public bool IsUrl { get; set; }
        public bool IsYoutubeUrl { get; set; }
        public bool IsWindowsMediaVideo { get; set; }
        public bool IsQuickTimeVideo { get; set; }
        public bool IsAviVideo { get; set; }
        public bool IsMsOfficeFile { get; set; }
        public bool IsImage { get; set; }
        public bool IsAcadFile { get; set; }

        public virtual string Display()
        {
            return "Hello world";
        }
    }
}