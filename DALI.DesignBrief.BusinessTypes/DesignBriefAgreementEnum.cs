using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.DesignBrief.BusinessTypes.Enums
{

    public enum DesignBriefAgreementEnum
    {
        UAVGC = 1,
        RAW = 2,
        TaskDescription = 3,
        Unknown = 4
    }

    public static class DesignBriefAgreementEnumExt
    {

        public static DesignBriefAgreementEnum AsEnum(int value)
        {
            return (DesignBriefAgreementEnum)Enum.ToObject(typeof(DesignBriefAgreementEnum), value);
        }

        public static int AsInt(this DesignBriefAgreementEnum value)
        {
            return (int)value;
        }
    }
}
