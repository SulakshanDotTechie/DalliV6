using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DALI.PolicyRequirements.Resources;

namespace DALI.Enums
{
    public class PublishingDecisionEnum
    {
        public const int Undescided = 0;
        public const int Approved = 1;
        public const int Parked = 3;
        public const int Rejected = 2;

        private static Dictionary<int, string> list;

        private static PublishingDecisionEnum status;

        public PublishingDecisionEnum()
        {
            if (null == list)
            {
                list = new Dictionary<int, string>();
                list.Add(Approved, Localization.Approved);
                list.Add(Rejected, Localization.Rejected);
            }
        }

        public static PublishingDecisionEnum Instance
        {
            get
            {
                if (null == status)
                    status = new PublishingDecisionEnum();

                return status;
            }
        }

        public Dictionary<int, string> List
        {
            get
            {
                return list;
            }
        }

        public int GetEnumKey(string enumValue)
        {
            return list.Where(l => l.Value == enumValue).FirstOrDefault().Key;
        }
    }
}
