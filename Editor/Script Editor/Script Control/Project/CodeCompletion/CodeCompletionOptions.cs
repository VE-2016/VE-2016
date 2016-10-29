
namespace AIMS.Libraries.Scripting.ScriptControl
{
    /// <summary>
    /// Class containing static properties for the code completion options.
    /// </summary>
    public static class CodeCompletionOptions
    {
        public static bool EnableCodeCompletion
        {
            get { return true; }
        }

        public static bool DataUsageCacheEnabled
        {
            get { return true; }
        }

        public static int DataUsageCacheItemCount
        {
            get { return 40; }
        }

        public static bool TooltipsEnabled
        {
            get { return true; }
        }

        public static bool TooltipsOnlyWhenDebugging
        {
            get { return false; }
        }

        public static bool KeywordCompletionEnabled
        {
            get { return true; }
        }

        public static bool InsightEnabled
        {
            get { return true; }
        }

        public static bool InsightRefreshOnComma
        {
            get { return true; }
        }
    }
}