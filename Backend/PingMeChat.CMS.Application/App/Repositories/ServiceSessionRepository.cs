using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore;

namespace PingMeChat.CMS.Application.App.Repositories
{
    public class ServiceSessionRepository : Repository<ServiceSession>, IServiceSessionRepository
    {
        public ServiceSessionRepository(AppDBContext context) : base(context)
        {
        }
    }
}
