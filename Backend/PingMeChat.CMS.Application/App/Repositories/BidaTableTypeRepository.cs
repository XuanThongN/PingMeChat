using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.App.Repositories
{
    public class BidaTableTypeRepository : Repository<BidaTableType>, IBidaTableTypeRepository
    {
        public BidaTableTypeRepository(AppDBContext context) : base(context)
        {
        } 
    }
}
