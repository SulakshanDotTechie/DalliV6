using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DALI.PolicyRequirements.Resources;

namespace DALI.Enums
{
    public class ChangeRequestDecisionEnum
    {
        public const int Approved = 1;
        public const int Rejected = 2;
        public const int Approve = 3;
        public const int Reject = 4;
        private static Dictionary<int, string> list;

        private static ChangeRequestDecisionEnum status;

        public ChangeRequestDecisionEnum()
        {
            if (null == list)
            {
                list = new Dictionary<int, string>();
                list.Add(Approved, Localization.ChangeRequestIsBeingProcessed);
                list.Add(Rejected, Localization.ChangeRequestIsNotBeingProcessed);
                list.Add(Approve, Localization.Approve);
                list.Add(Reject, Localization.Reject);
            }
        }

        public static ChangeRequestDecisionEnum Instance
        {
            get
            {
                if (null == status)
                    status = new ChangeRequestDecisionEnum();

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
