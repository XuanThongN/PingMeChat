using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Entities;
using PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore;

namespace PingMeChat.CMS.Application.App.Repositories
{
    public class GroupRoleRepository : Repository<GroupRole>, IGroupRoleRepository
    {
        public GroupRoleRepository(AppDBContext context) : base(context)
        {
        }
    }
}
