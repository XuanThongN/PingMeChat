using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Entities;
using PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.App.Repositories
{
    public class MenuRepository : Repository<Menu>, IMenuRepository
    {
        public MenuRepository(AppDBContext context) : base(context)
        {
        }

      
    }
}
