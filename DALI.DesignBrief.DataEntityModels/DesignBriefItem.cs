using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.DesignBrief.DataEntityModels
{
    public partial class DesignBriefItem : IDesignBrief
    {
        public string TableName
        {
            get
            {
                return "DesignBriefItems";
            }
        }

        public string SemiPathKey
        {
            get
            {
                return string.Format("{0}_{1}_{2}_{3}_{4}", ChapterId, LevelId, LocationId, AreaId, SubjectId);
            }
        }
    }
}
