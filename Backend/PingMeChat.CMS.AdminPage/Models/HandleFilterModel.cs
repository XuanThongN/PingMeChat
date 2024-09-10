namespace PingMeChat.CMS.AdminPage.Models
{
    public class HandleFilterModel
    {
        public bool IsSearch { get; set; }
        public bool IsReset { get; set; }

        public HandleFilterModel(bool isSearch, bool isReset)
        {
            IsSearch = isSearch;
            IsReset = isReset;
        }
    }
}
