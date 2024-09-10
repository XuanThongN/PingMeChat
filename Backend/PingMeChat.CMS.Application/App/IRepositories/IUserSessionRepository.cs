using PingMeChat.CMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.App.IRepositories
{
    public interface IUserSessionRepository : IRepository<UserSession>
    {
        Task LogoutAllSessionsForUserAsync(string userId);
    }
}
