using DALI.Topics.Infrastructure.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.DesignBrief.DomainModels.Models
{
    public class DesignBriefThemeReferenceModel
    {
        public DesignBriefThemeReferenceModel()
        {
            Theme = new DesignBriefThemeModel();
            DesignBriefItemIds = new List<int>();
        }

        public DesignBriefThemeModel Theme { get; }

        public List<int> DesignBriefItemIds { get; }

        public TopicResponseModel Results
        {
            get;
            set;
        }
    }
}
