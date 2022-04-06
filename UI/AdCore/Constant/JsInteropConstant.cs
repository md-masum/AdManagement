namespace AdCore.Constant
{
    public static class JsInteropConstant
    {
        private const string FuncPrefix = "adManagement";

        public const string GetSessionStorage = $"{FuncPrefix}.getSessionStorage";
        public const string SetSessionStorage = $"{FuncPrefix}.setSessionStorage";
        public const string RemoveSessionStorage = $"{FuncPrefix}.removeSessionStorage";
        public const string ScrollToBottom = $"{FuncPrefix}.scrollToBottom";

        public const string PlatNotification = $"{FuncPrefix}.playNotification";


        #region Session Key

        public const string CurrentUserId = "currentUserId";

        #endregion
    }
}
