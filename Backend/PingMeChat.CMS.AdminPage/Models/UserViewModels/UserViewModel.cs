namespace PingMeChat.CMS.AdminPage.Models.UserViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? AvatarURL { get; set; }
        public string? DateRegistered { get; set; }
        public string? Position { get; set; }
        public string? NickName { get; set; }
    }
}
