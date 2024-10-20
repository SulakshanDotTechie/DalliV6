using System;
using System.Collections.Generic;
using System.Resources;

namespace DALI.DesignBrief.BusinessTypes.Enums
{
    public enum DesignBriefStatusEnum
    {
        None = 0,
        Concept = 1,
        Secured = 2,
        Finished = 3,
        Canceled = 4
    }

    public static class DesignBriefStatusEnumExt
    {
        public static string AsDisplayString(this DesignBriefStatusEnum status)
        {
            switch (status)
            {
                case DesignBriefStatusEnum.None: return string.Empty;
                case DesignBriefStatusEnum.Concept: return Resources.Localization.Concept;
                case DesignBriefStatusEnum.Secured: return Resources.Localization.Secured;
                case DesignBriefStatusEnum.Finished: return Resources.Localization.Finished;
                case DesignBriefStatusEnum.Canceled: return Resources.Localization.Canceled;
            }

            return string.Empty;
        }

        public static string AsDisplayString(int value)
        {
            return AsEnum(value).AsDisplayString();
        }

        public static DesignBriefStatusEnum AsEnum(int value)
        {
            return (DesignBriefStatusEnum)Enum.ToObject(typeof(DesignBriefStatusEnum), value);
        }

        public static int AsInt(this DesignBriefStatusEnum value)
        {
            return (int)value;
        }

        public static SortedDictionary<int, string> ResourceNames
        {
            get
            {
                SortedDictionary<int, string> _ResourceNames = new SortedDictionary<int, string>();

                foreach (DesignBriefStatusEnum statusEnum in Enum.GetValues(typeof(DesignBriefStatusEnum)))
                {
                    int status = statusEnum.AsInt();
                    if (_ResourceNames.ContainsKey(status))
                        _ResourceNames.Add(status, statusEnum.AsDisplayString());
                }

                return _ResourceNames;
            }
        }
    }
}
