namespace PingMeChat.CMS.AdminPage.Models
{
    public class FormHeaderViewModel
    {
        public string Title { get; set; }
        public bool HasCreateNew { get; set; }
        public bool HasBack { get; set; }
        public FormHeaderViewModel(string title)
        {
            Title = title;
        }
        public FormHeaderViewModel(string title, bool hasCreateNew, bool hasBack)
        {
            Title = title;
            HasCreateNew = hasCreateNew;
            HasBack = hasBack;
        }
    }
}
