using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Entities;
using PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore;

namespace PingMeChat.CMS.Application.App.Repositories
{
    public class UsersMenuRepository : Repository<UserMenu>, IUsersMenuRepository
    {
        public UsersMenuRepository(AppDBContext context) : base(context)
        {
        }
    }
}
