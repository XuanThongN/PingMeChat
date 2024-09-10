namespace PingMeChat.Shared
{
    public static class HandleMessage
    {
        public static string GetMessageByCode(string codeMessage, params string[] args)
        {
            if (args != null && args.Length > 0)
            {
                return string.Format(codeMessage, args);
            }
            return codeMessage;
        }
    }
}
