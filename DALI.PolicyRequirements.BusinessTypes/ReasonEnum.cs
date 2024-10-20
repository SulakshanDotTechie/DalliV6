using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DALI.PolicyRequirements.Resources;

namespace DALI.Enums
{
    public class ReasonEnum
    {
        public static readonly int Add = 1;
        public static readonly int Edit = 2;
        public static readonly int Remove = 3;
        private static Dictionary<int, string> list;

        private static ReasonEnum reason;

        public ReasonEnum()
        {
            if (null == list)
            {
                list = new Dictionary<int, string>();
                list.Add(Add, Localization.Add);
                list.Add(Edit, Localization.Edit);
                list.Add(Remove, Localization.Remove);
            }
        }

        public static ReasonEnum Instance
        {
            get
            {
                if (null == reason)
                    reason = new ReasonEnum();

                return reason;
            }
        }

        public Dictionary<int, string> List
        {
            get
            {
                return list;
            }
        }

        public int GetEnumKey(int enumKey)
        {
            return list.Where(l => l.Key == enumKey).FirstOrDefault().Key;
        }
    }
}
