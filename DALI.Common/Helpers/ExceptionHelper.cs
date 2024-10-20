using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALI.Common.Helpers
{
    public static class ExceptionHelper
    {
        public static string ErrorMessageHandler(Exception error, string preFixMsg, string postFixMsg)
        {
            string errorMessage = string.Empty;
            if (null != error.InnerException && error.InnerException is SqlException)
            {
                SqlException sqlError = error.InnerException as SqlException;
                switch (sqlError.Number)
                {
                    case 2627:
                        errorMessage = DALI.Common.Resources.Localization.ErrorDuplicateValue;
                        break;
                    case 2601:
                        errorMessage = DALI.Common.Resources.Localization.ErrorDuplicateValue;
                        break;
                    case 515:
                        errorMessage = sqlError.Errors[0].Message;
                        break;
                    default:
                        errorMessage = error.Message;
                        break;
                }
            }
            else
            {
                errorMessage = error.Message;
            }

            if (!string.IsNullOrEmpty(preFixMsg))
                errorMessage = preFixMsg + " " + errorMessage;

            if (!string.IsNullOrEmpty(preFixMsg))
                errorMessage = errorMessage + " " + postFixMsg;

            return errorMessage;
        }

        public static string ErrorMessageHandler(Exception error)
        {
            return ErrorMessageHandler(error, string.Empty, string.Empty);
        }
    }
}
