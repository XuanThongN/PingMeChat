namespace PingMeChat.CMS.AdminPage.Models
{
    public class FormUploadFileModel
    {
        public string Url { get; set; }
        public string Title { get; set; }

        public FormUploadFileModel(string url, string title)
        {
            Url = url;
            Title = title;
        }
    }
}
