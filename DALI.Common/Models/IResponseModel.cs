using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.Models
{
    public interface IResponseModel
    {
        int Status { get; set; }
        Dictionary<string, string> Errors { get; set; }
    }
}
