using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Progetto.Web.Infrastructure
{
    public static class TempDataExtensions
    {
        private const string SuccessKey = "SuccessMessage";
        private const string ErrorKey = "ErrorMessage";
        private const string InfoKey = "InfoMessage";
        private const string WarningKey = "WarningMessage";

        public static void Success(this ITempDataDictionary tempData, string message)
        {
            tempData[SuccessKey] = message;
        }

        public static void Error(this ITempDataDictionary tempData, string message)
        {
            tempData[ErrorKey] = message;
        }

        public static void Info(this ITempDataDictionary tempData, string message)
        {
            tempData[InfoKey] = message;
        }

        public static void Warning(this ITempDataDictionary tempData, string message)
        {
            tempData[WarningKey] = message;
        }

        public static string GetSuccess(this ITempDataDictionary tempData)
        {
            return tempData[SuccessKey]?.ToString();
        }

        public static string GetError(this ITempDataDictionary tempData)
        {
            return tempData[ErrorKey]?.ToString();
        }

        public static string GetInfo(this ITempDataDictionary tempData)
        {
            return tempData[InfoKey]?.ToString();
        }

        public static string GetWarning(this ITempDataDictionary tempData)
        {
            return tempData[WarningKey]?.ToString();
        }
    }
}
