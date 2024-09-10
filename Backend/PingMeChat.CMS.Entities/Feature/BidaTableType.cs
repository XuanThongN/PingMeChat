using PingMeChat.CMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Feature
{
    public  class BidaTableType : AuditableBaseEntity
    {
        public BidaTableType()
        {
            BidaTables = new HashSet<BidaTable>();
        }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public virtual IEnumerable<BidaTable>? BidaTables { get; set; }
    }
}
