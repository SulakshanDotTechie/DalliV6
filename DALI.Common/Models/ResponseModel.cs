using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace DALI.Models
{
    [Serializable]
    public class ResponseModel : IResponseModel
    {
        public int Id { get; set; }

        private Dictionary<string, string> _Errors { get; set; }

        public int Status { get; set; }

        public string Content { get; set; }

        public Dictionary<string, string> Errors
        {
            get
            {
                if (_Errors == null)
                    _Errors = new Dictionary<string, string>();

                return _Errors;
            }
            set
            {
                _Errors = value;
            }
        }

        public dynamic Model
        {
            get;
            set;
        }
    }
}
