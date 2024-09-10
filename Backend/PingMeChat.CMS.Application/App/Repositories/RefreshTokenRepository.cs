using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Entities;
using PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore;

namespace PingMeChat.CMS.Application.App.Repositories
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDBContext context) : base(context)
        {
        }
    }
}
