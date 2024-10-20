using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.Enums
{
    public enum AssignmentEnum
    {
        Strictness = 1,
        Attachment = 2,
        SourceDocument = 3
    }

    public static class AssignmentEnumExt
    {
        public static AssignmentEnum AsEnum(int value)
        {
            return (AssignmentEnum)Enum.ToObject(typeof(AssignmentEnum), value);
        }

        public static int AsInt(this AssignmentEnum value)
        {
            return (int)value;
        }
    }
}
