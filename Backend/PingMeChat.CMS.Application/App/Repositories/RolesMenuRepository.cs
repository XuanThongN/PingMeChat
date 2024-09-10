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
    public class RolesMenuRepository : Repository<RoleMenu>, IRolesMenuRepository
    {
        public RolesMenuRepository(AppDBContext context) : base(context)
        {
        }
    }
}
