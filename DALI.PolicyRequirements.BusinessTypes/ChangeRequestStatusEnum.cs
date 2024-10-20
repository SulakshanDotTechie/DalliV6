using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DALI.PolicyRequirements.Resources;

namespace DALI.Enums
{
    public class ChangeRequestStatusEnum
    {
        public static readonly int New = 1;
        public static readonly int InProgress = 2;
        public static readonly int Completed = 3;
        public static readonly int Rejected = 4;
        public static readonly int Final = 5;
        public static readonly int RejectedByPublisher = 6;

        private static Dictionary<int, string> list;
        private static Dictionary<int, string> tooltip;
        private static Dictionary<int, string> mailSubjects;

        private static ChangeRequestStatusEnum status;

        public ChangeRequestStatusEnum()
        {
            if (null == list)
            {
                list = new Dictionary<int, string>();
                list.Add(New, Localization.New);
                list.Add(InProgress, Localization.InProgress);
                list.Add(Completed, Localization.Completed);
                list.Add(Rejected, Localization.InprogressRejected);
                list.Add(Final, Localization.ChangesAreFinalAndReadyForPublishing);
                list.Add(RejectedByPublisher, Localization.RejectedByPublisher);
            }

            if (null == tooltip)
            {
                tooltip = new Dictionary<int, string>();
                tooltip.Add(New, Localization.TooltipForNewChangeRequest);
                tooltip.Add(InProgress, Localization.TooltipForChangeRequestInProgress);
                tooltip.Add(Completed, Localization.TooltipForChangeRequestCompleted);
                tooltip.Add(Rejected, Localization.TooltipForChangeRequestRejected);
                tooltip.Add(Final, Localization.TooltipForChangeRequestFinal);
                tooltip.Add(RejectedByPublisher, Localization.TooltipForChangeRequestRejectedByPublisher);
            }

            if (mailSubjects == null)
            {
                mailSubjects = new Dictionary<int, string>();
                mailSubjects.Add(New, Localization.New);
                mailSubjects.Add(InProgress, Localization.ChangeRequestIsBeingProcessed);
                mailSubjects.Add(Completed, Localization.Completed);
                mailSubjects.Add(Rejected, Localization.ChangeRequestIsNotBeingProcessed);
                mailSubjects.Add(Final, Localization.ChangesAreFinalAndReadyForPublishing);
                mailSubjects.Add(RejectedByPublisher, Localization.RejectedByPublisher);
            }
        }

        public static ChangeRequestStatusEnum Instance
        {
            get
            {
                if (null == status)
                    status = new ChangeRequestStatusEnum();

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

        public Dictionary<int, string> Tooltip
        {
            get
            {
                return tooltip;
            }
        }

        public Dictionary<int, string> MailSubjects
        {
            get
            {
                return mailSubjects;
            }
        }

        public int GetEnumKey(string enumValue)
        {
            return list.Where(l => l.Value == enumValue).FirstOrDefault().Key;
        }
    }
}
