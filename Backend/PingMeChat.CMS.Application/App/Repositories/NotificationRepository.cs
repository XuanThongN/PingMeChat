using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.CMS.Entities.Module;
using PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.App.Repositories
{
    public class NotificationRepository : Repository<Notification> , INotificationRepository
    {
        public NotificationRepository(AppDBContext dbContext) : base(dbContext)
        {
        }
    }
}
