using PingMeChat.CMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Feature
{
    public class Voucher : AuditableBaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
