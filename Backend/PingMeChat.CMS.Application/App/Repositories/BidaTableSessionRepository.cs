using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.App.Repositories
{
    public class BidaTableSessionRepository : Repository<BidaTableSession>, IBidaTableSessionRepository
    {
        public BidaTableSessionRepository(AppDBContext context) : base(context)
        {
        }
    }
}
